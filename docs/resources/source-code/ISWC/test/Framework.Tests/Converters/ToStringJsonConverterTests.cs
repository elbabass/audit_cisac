using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace SpanishPoint.Azure.Iswc.Framework.Tests.Converters
{
    /// <summary>
    /// Tests TConverter
    /// </summary>
    public class ToStringJsonConverterTests
    {
        /// <summary>
        /// Test succeeds if CanConvert returns true
        /// </summary>
        [Fact]
        public void CanConvert_test()
        {
            var toStringJsonConverter = new ToStringJsonConverter();
            bool value = toStringJsonConverter.CanConvert(typeof(string));
            Assert.True(value);
        }

        /// <summary>
        /// Test succeeds if value is returned as expected
        /// </summary>
        [Fact]
        public void WriteJson_Test()
        {
            var serialiser = new JsonSerializer();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            var writer = new JsonTextWriter(sw);

            var toStringJsonConverter = new ToStringJsonConverter();
            toStringJsonConverter.WriteJson(writer, "test", serialiser);
            var test = sb.ToString();

            Assert.Equal("\"test\"", test);
        }

        /// <summary>
        /// Tests succeeds if CanRead returns false
        /// </summary>
        [Fact]
        public void CanRead_Test()
        {
            var toStringJsonConverter = new ToStringJsonConverter();
            bool value = toStringJsonConverter.CanRead;
            Assert.False(value);
        }

        /// <summary>
        /// Tests succeeds if an excepetion is returned
        /// </summary>
        [Fact]
        public void ReadJson_Test()
        {
            var serialiser = new JsonSerializer();
            var reader = CreateJsonReader("10");
            var toStringJsonConverter = new ToStringJsonConverter();

            Action test = () => { toStringJsonConverter.ReadJson(reader, typeof(string), null, serialiser); };
            
            var ex = Record.Exception(test);
            Assert.NotNull(ex);
            Assert.IsType<NotImplementedException>(ex);
        }

        private JsonTextReader CreateJsonReader(string json)
            => new JsonTextReader(new StringReader(json));
    }
}
