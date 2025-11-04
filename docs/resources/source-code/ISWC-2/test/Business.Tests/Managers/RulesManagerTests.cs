using Microsoft.Extensions.Caching.Memory;
using Moq;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Services.Rules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Business.Tests.Managers
{
    /// <summary>
    /// Tests RuleManager
    /// </summary>
    public class RulesManagerTests
    {
        private readonly Mock<IMemoryCache> memoryCache = new Mock<IMemoryCache>();
        private readonly Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
        private readonly Mock<IRulesService> rulesService = new Mock<IRulesService>();

        private RulesManager GetManager()
        {
            return new Mock<RulesManager>(serviceProvider.Object, rulesService.Object, memoryCache.Object).Object;
        }

        /// <summary>
        /// Check deserialize returns string
        /// </summary>
        [Fact]
        public async Task Deserialize_Returns_String()
        {
            var parameter = "param1";
            var value = "val1";
            
            object expected = new Dictionary<string, string> { { parameter, value } };
            memoryCache.Setup(x => x.TryGetValue("Rules", out expected)).Returns(true);

            var manager = GetManager();

            Assert.Equal(value, await manager.GetParameterValue<string>(parameter));
        }

        /// <summary>
        /// Check deserialize returns int
        /// </summary>
        [Fact]
        public async Task Deserialize_Returns_Int()
        {
            var parameter = "param1";
            var value = "1";

            object expected = new Dictionary<string, string> { { parameter, value } };
            memoryCache.Setup(x => x.TryGetValue("Rules", out expected)).Returns(true);
            
            var manager = GetManager();

            Assert.Equal(1, await manager.GetParameterValue<int>(parameter));
        }

        /// <summary>
        /// Check deserialize returns bool
        /// </summary>
        [Fact]
        public async Task Deserialize_Returns_Bool()
        {
            var parameterTrue = "paramTrue";
            var parameterFalse = "paramFalse";
            var valueTrue = "true";
            var valueFalse = "false";

            object expected = new Dictionary<string, string> { { parameterTrue, valueTrue }, { parameterFalse, valueFalse } };
            memoryCache.Setup(x => x.TryGetValue("Rules", out expected)).Returns(true);
            
            var manager = GetManager();

            Assert.True(await manager.GetParameterValue<bool>(parameterTrue));
            Assert.False(await manager.GetParameterValue<bool>(parameterFalse));
        }

        /// <summary>
        /// Check deserialize returns list of strings
        /// </summary>
        [Fact]
        public async Task DeserializeEnumerable_Returns_ListOfStrings()
        {
            var parameter = "param1";
            var value = "str1,str2";

            object expected = new Dictionary<string, string> { { parameter, value } };
            memoryCache.Setup(x => x.TryGetValue("Rules", out expected)).Returns(true);

            var manager = GetManager();

            Assert.Equal(new string[] { "str1", "str2" },
                await manager.GetParameterValueEnumerable<string>(parameter));
        }

        /// <summary>
        /// Check deserialize returns list of enums
        /// </summary>
        [Fact]
        public async Task DeserializeEnumerable_Returns_ListOfEnums()
        {
            var parameter = "param1";
            var value = "CAR,CUR";

            object expected = new Dictionary<string, string> { { parameter, value } };
            memoryCache.Setup(x => x.TryGetValue("Rules", out expected)).Returns(true);

            var manager = GetManager();

            Assert.Equal(new TransactionType[] { TransactionType.CAR, TransactionType.CUR },
                await manager.GetParameterValueEnumerable<TransactionType>(parameter));
        }
    }
}
