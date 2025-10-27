using Xunit;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Net.Http;

namespace SpanishPoint.Azure.Iswc.Framework.Tests.Extensions
{
    /// <summary>
    /// Tests HttpResponseMessageExtensions
    /// </summary>
    public class HttpResponseMessageExtensionsTest
    {
        /// <summary>
        /// HttpResponseMessage Extension test returns successful response when isSuccessStatus Code is true
        /// </summary>
        [Fact]
        public void EnsureSuccessStatusCodeAsync_Test1()
        {
            var message = new HttpResponseMessage();
            var response = HttpResponseMessageExtensions.EnsureSuccessStatusCodeAsync(message);
            Assert.NotNull(response);
            Assert.True(response.IsCompletedSuccessfully);
        }

        /// <summary>
        /// HttpResponseMessage Extension test returns false response when statusCode is not between 200-299
        /// </summary>
        [Fact]
        public void EnsureSuccessStatusCodeAsync_Test2()
        {
            var message = new HttpResponseMessage() {
                StatusCode = (System.Net.HttpStatusCode)301
            };

            var response = HttpResponseMessageExtensions.EnsureSuccessStatusCodeAsync(message);
            Assert.NotNull(response);
            Assert.False(response.IsCompletedSuccessfully);
        }
    }
}
