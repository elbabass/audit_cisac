using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test STN Deserializer
    /// </summary>
    public class STNTest
    {
        /// <summary>
        /// STN Deserializer - Test the STN record type deserializes correctly from string
        /// </summary>
        [Fact]
        public void Deserialize_STN_Record()
        {
            // Arrange
            var stnRecord = "STN0000000000000002I-003305919-51201103311403369999123123595920110331140336";

            var expectedIpBaseNumber = "I-003305919-5";
            var expectedStatusCode = 1;
            var expectedValidFrom = DateTime.ParseExact("20110331140336", "yyyyMMddHHmmss", null);
            var expectedValidTo = DateTime.ParseExact("99991231235959", "yyyyMMddHHmmss", null);
            var expectedAmendDate = DateTime.ParseExact("20110331140336", "yyyyMMddHHmmss", null);


            // Act
            var stn = new STN(stnRecord);

            // Assert
            Assert.Equal(expectedIpBaseNumber, stn.IPBaseNumber);
            Assert.Equal(expectedStatusCode, (int)stn.StatusCode);
            Assert.Equal(expectedValidFrom, stn.ValidFrom);
            Assert.Equal(expectedValidTo, stn.ValidTo);
            Assert.Equal(expectedAmendDate, stn.AmendmentDate);
        }
    }
}