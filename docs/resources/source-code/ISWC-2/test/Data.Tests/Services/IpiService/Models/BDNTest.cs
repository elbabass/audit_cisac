using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test BDN Deserializer
    /// </summary>
    public class BDNTest
    {
        /// <summary>
        /// BDN Deserializer - Test the BDN record type deserializes correctly from string input with a null date
        /// </summary>
        [Fact]
        public void Deserialize_BDN_Record_NullDeathDate()
        {
            // Arrange
            string bdnRecord = "BDN0000000000000001L                                                                                                                     20110331140336";

            DateTime? expectedBirthDate = null;
            DateTime? expectedDeathDate = null;
            string expectedSex = null;
            string expectedStateOfBirth = null;
            string expectedPlaceOfBirth = null;
            DateTime? expectedAmendedDateTime = DateTime.ParseExact("20110331140336", "yyyyMMddHHmmss", null);
            string expectedType = InterestedPartyType.L.ToString();


            // Act
            var bdn = new BDN(bdnRecord);

            // Assert
            Assert.Equal(expectedBirthDate, bdn.GetDateOfBirth());
            Assert.Equal(expectedDeathDate, bdn.GetDateOfDeath());
            Assert.Equal(expectedSex, bdn.Sex);
            Assert.Equal(expectedStateOfBirth, bdn.StateOfBirth);
            Assert.Equal(expectedPlaceOfBirth, bdn.PlaceOfBirth);
            Assert.Equal(expectedAmendedDateTime, bdn.AmendedDate);
            Assert.Equal(expectedType, bdn.InterestedPartyType.ToString());

        }

        /// <summary>
        /// BDN Deserializer - Test the BDN record type deserializes correctly from string input with a valid date
        /// </summary>
        [Fact]
        public void Deserialize_BDN_Record_FullDate()
        {
            // Arrange
            string bdnRecord = "BDN0000000000000001L        20190425                                                                                                     20110331140336";

            DateTime? expectedDeathDate = DateTime.ParseExact("20190425", "yyyyMMdd", null);
            InterestedPartyType expectedType = InterestedPartyType.L;


            // Act
            var bdn = new BDN(bdnRecord);

            // Assert
            Assert.Equal(expectedDeathDate, bdn.GetDateOfDeath());
            Assert.Equal(expectedType, bdn.InterestedPartyType);
        }
    }
}
