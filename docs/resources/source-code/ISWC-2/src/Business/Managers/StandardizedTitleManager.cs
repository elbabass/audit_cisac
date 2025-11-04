using AutoMapper;
using Microsoft.Azure.Search.Models;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IStandardizedTitleManager
    {
        Task<IEnumerable<StandardizedTitle>> FindManyAsync(IEnumerable<string> wordsToReplace);
    }

    public class StandardizedTitleManager : IStandardizedTitleManager
    {
        private readonly IStandardizedTitleRepository standardizedTitleRepository;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;

        public StandardizedTitleManager(IStandardizedTitleRepository standardizedTitleRepository, IMapper mapper, ICacheClient cacheClient)
        {
            this.standardizedTitleRepository = standardizedTitleRepository;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
        }

        public async Task<IEnumerable<StandardizedTitle>> FindManyAsync(IEnumerable<string> wordsToReplace)
        {
            var standardizedTitles = await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<IEnumerable<StandardizedTitle>>(await standardizedTitleRepository.FindManyAsync(x => true));
            });

            return standardizedTitles.Where(x => wordsToReplace.Contains(x.ReplacePattern)).DistinctBy(x => x.ReplacePattern);
        }
    }
}
