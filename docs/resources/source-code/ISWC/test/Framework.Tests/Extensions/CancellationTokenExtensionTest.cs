using System.Threading;
using Xunit;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Threading.Tasks;
using System;

namespace SpanishPoint.Azure.Iswc.Framework.Tests.Extensions
{
    /// <summary>
    /// Tests CancellationToken
    /// </summary>
    public class CancellationTokenExtensionTest
    {
        /// <summary>
        /// Test succeeds if an exception is returned
        /// </summary>
        [Fact]
        public void CancellationToken_Valid()
        {
            CancellationToken? token = new CancellationToken(true);
            Action test = () => { token.ThrowIfCancelled(); };
            var ex = Record.Exception (test);
            Assert.NotNull(ex);
            Assert.IsType<TaskCanceledException>(ex);
        }
    }
}
