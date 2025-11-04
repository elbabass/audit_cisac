using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Framework.Tests.Extensions
{
    /// <summary>
    /// Tests StringExtensions
    /// </summary>
    public class StringExtensionsTests
    {
        /// <summary>
        /// Check deserialize returns string
        /// </summary>
        [Fact]
        public void Deserialize_Returns_String()
        {
            var parameter = "teststring";
            Assert.Equal(parameter, parameter.Deserialize<string>());
        }

        /// <summary>
        /// Check deserialize returns int
        /// </summary>
        [Fact]
        public void Deserialize_Returns_Int()
        {
            var parameter = "1";
            Assert.Equal(1, parameter.Deserialize<int>());
        }

        /// <summary>
        /// Check deserialize returns bool
        /// </summary>
        [Fact]
        public void Deserialize_Returns_Bool()
        {
            var parameterTrue = "true";
            var parameterFalse = "false";
            Assert.True(parameterTrue.Deserialize<bool>());
            Assert.False(parameterFalse.Deserialize<bool>());
        }

        /// <summary>
        /// Check deserialize returns list of strings
        /// </summary>
        [Fact]
        public void DeserializeEnumerable_Returns_ListOfStrings()
        {
            var parameter = "str1,str2";
            var list = parameter.DeserializeEnumerable<string>();
            Assert.Equal(new string[] { "str1", "str2" }, list);
        }

        /// <summary>
        /// Check deserialize returns list of enums
        /// </summary>
        [Fact]
        public void DeserializeEnumerable_Returns_ListOfEnums()
        {
            var parameter = "CAR,CUR";
            var list = parameter.DeserializeEnumerable<TransactionType>();
            Assert.Equal(new TransactionType[] { TransactionType.CAR, TransactionType.CUR }, list);
        }

        /// <summary>
        /// Check Increment Iswc returns next Iswc
        /// </summary>
        /// <param name="iswc"></param>
        /// <param name="nextIswc"></param>
        [Theory]
        [InlineData("T0345246801", "T0345246812")]
        [InlineData("T9066996245", "T9066996256")]
        [InlineData("T2030000100", "T2030000111")]
        [InlineData("T9277882323", "T9277882334")]
        [InlineData("T9800017134", "T9800017145")]
        public void IncrementIswc_Returns_NextIswc(string iswc, string nextIswc)
        {
            Assert.Equal(nextIswc, iswc.IncrementIswc());
        }

        /// <summary>
        /// Check TrimStart removes leading zero for agency code
        /// </summary>
        /// <param name="expectedCode"></param>
        /// <param name="testCode"></param>
        [Theory]
        [InlineData("21", "021")]
        [InlineData("1", "001")]
        [InlineData("21", "21")]
        public void TrimStart_TrimsZerosForAgencyCode(string expectedCode, string testCode)
        {
            Assert.Equal(expectedCode, testCode.TrimStart('0'));
        }

        /// <summary>
        /// Check Iswc pattern is correct
        /// </summary>
        /// <param name="iswc"></param>
        [Theory]
        [InlineData("T0345246801")]
        [InlineData("T9066996245")]
        [InlineData("T2030000100")]
        [InlineData("T9277882323")]
        [InlineData("T9800017134")]
        public void Iswc_Pattern_Correct(string iswc)
        {
            Assert.True(iswc.IsValidIswcPattern());
        }

        /// <summary>
        /// Check Iswc pattern is incorrect
        /// </summary>
        /// <param name="iswc"></param>
        [Theory]
        [InlineData("0345246801")]
        [InlineData("Ts9066996245f34")]
        [InlineData("T20300sdf00100")]
        [InlineData("T92778823")]
        [InlineData("T980001713")]
        public void Iswc_Pattern_Incorrect(string iswc)
        {
            Assert.False(iswc.IsValidIswcPattern());
        }

        /// <summary>
        /// Check string is sperated by ;
        /// </summary>
        [Fact]
        public void CreateSemiColonSperatedString_Returns_SperatedString()
        {
            var values = new List<string> { "1", "2", "4" };
            var result = StringExtensions.CreateSemiColonSeperatedString(values);
            
            Assert.Equal("1;2;4", result);
        }
    }
}
