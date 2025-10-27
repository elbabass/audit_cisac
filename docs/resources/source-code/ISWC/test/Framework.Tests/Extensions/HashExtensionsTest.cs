using SpanishPoint.Azure.Iswc.Framework.Extensions;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Framework.Tests.Extensions
{
    /// <summary>
    /// Tests HashExtensions
    /// </summary>
    public class HashExtensionsTest
    {
        /// <summary>
        /// MD5Hash Function returns valid hash value
        /// </summary>
        [Theory]
        [InlineData("ab49a07ba78c11cb19d09fa3940c43e3", "TEST TITLE 1279271434136730972DISAMBIGUATION TEST 1DISAMBIGUATION TEST 2TrueDITB")]
        [InlineData("738b8cc4ca3a8bf17aec79b3921f79e5", "haiudf duah9sdnasjd fkahof hau sd")]
        public void Validate_MD5Hash_True(string expected, string toHash)
        {
            Assert.Equal(expected, toHash.GetHashValue());
        }

        /// <summary>
        /// MD5Hash Function returns valid hash value
        /// </summary>
        [Theory]
        [InlineData("ab49a07ba78c11cb19d09fa3940c43e3", "TEST TITLE 1279271434136730972DISAMBIGUATION TEST 1TrueDITB")]
        [InlineData("1234", "haiudf duah9sdnasjd fkahof hau sd")]
        public void Validate_MD5Hash_False(string expected, string toHash)
        {
            Assert.NotEqual(expected, toHash.GetHashValue());
        }
    }
}
