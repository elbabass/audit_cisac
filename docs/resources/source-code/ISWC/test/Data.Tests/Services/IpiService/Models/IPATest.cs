using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test IPA Deserializer
    /// </summary>
    public class IPATest
    {
        /// <summary>
        /// IPA Deserializer - Test the IPA record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_IPA_Record()
        {
            // Arrange          
            string ipaRecord = "IPA000000000000000020110331140336080SUISA               I-003305919-5100372918926PAI-003305919-5100372918926PA";

            var expectedIpBaseNumber = "I-003305919-5";
            var expectedAgency = "080";
            var expectedIPNameNumber = 372918926;
            var expectedCreatedDate = DateTime.ParseExact("20110331140336", "yyyyMMddHHmmss", null);
           
            // Act
            var ipa = new IPA(ipaRecord);

            // Assert
            Assert.Equal(expectedIpBaseNumber, ipa.IpBaseNumber);
            Assert.Equal(expectedAgency, ipa.RemittingSocietyCode);
            Assert.Equal(expectedIPNameNumber, ipa.IpNameNumber);
            Assert.Equal(expectedCreatedDate, ipa.TransactionDate);

        }
    }
}
