using Microsoft.Extensions.Caching.Memory;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Data.Services.Rules;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IRulesManager
    {
        Task<IEnumerable<T>> GetEnabledRules<T>() where T : IBaseRule;
        Task<T> GetParameterValue<T>(string parameterName);
        Task<IEnumerable<T>> GetParameterValueEnumerable<T>(string parameterName);
    }

    public class RulesManager : IRulesManager
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IRulesService rulesService;
        private readonly IMemoryCache memoryCache;

        public RulesManager(
            IServiceProvider serviceProvider,
            IRulesService rulesService,
            IMemoryCache memoryCache)
        {
            this.serviceProvider = serviceProvider;
            this.rulesService = rulesService;
            this.memoryCache = memoryCache;
        }

        public async Task<IEnumerable<T>> GetEnabledRules<T>() where T : IBaseRule
        {
            var rules = await GetRules();
            var ruleInstances = new List<T>();

            var allRuleTypes = AppDomain.CurrentDomain.GetAllLoadedAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x => typeof(T).IsAssignableFrom(x) && x.IsClass);

            foreach (var rule in allRuleTypes)
            {
                ruleInstances.Add((T)serviceProvider.GetService(rule));
            }

            return ruleInstances.Where(x => typeof(IAlwaysOnRule).IsAssignableFrom(x.GetType()) || rules.Any(b => b.Key.Contains(x.ParameterName))).OrderBy(r => r.Identifier);
        }

        public async Task<T> GetParameterValue<T>(string parameterName)
        {
            var value = (await GetRules())
                .FirstOrDefault(r => r.Key == parameterName)
                .Value;

            if (string.IsNullOrEmpty(value))
                return default!;
            else
                return value.Deserialize<T>();
        }

        public async Task<IEnumerable<T>> GetParameterValueEnumerable<T>(string parameterName)
        {
            var value = (await GetRules())
                .FirstOrDefault(r => r.Key == parameterName)
                .Value;
            if (string.IsNullOrEmpty(value))
                return default!;
            else
                return value.DeserializeEnumerable<T>();
        }

        private async Task<IDictionary<string, string>> GetRules() =>
            await memoryCache.GetOrCreateAsync("Rules", async rules =>
            {
                rules.Size = 10;
                rules.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                var enabledRules = (await rulesService.GetEnabledRules())?.Where(r => !string.IsNullOrEmpty(r.Value));
                return enabledRules.ToDictionary(x => x.Name, y => y.Value);
            });
    }
}