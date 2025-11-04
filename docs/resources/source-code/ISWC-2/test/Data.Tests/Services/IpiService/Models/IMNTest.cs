using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test IMN Deserializer
    /// </summary>
    public class IMNTest
    {
        /// <summary>
        /// IMN Deserializer - Test the IMN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_IMN_Record_NullDeathDate()
        {
            // Arrange
            string imnRecord = "IMN000000350000000700466171448MWLYI-002522430-8                                                                                                                                                               ";

            int expectedIpNameNumber = 00466171448;
            var expectedCreationClass = "MW";
            var expectedRole = "LY";
            var expectedIPBaseNumber = "I-002522430-8";


            // Act
            var imn = new IMN(imnRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, imn.IpNameNumber);
            Assert.Equal(expectedCreationClass, imn.CreationCode);
            Assert.Equal(expectedRole, imn.RoleCode);
            Assert.Equal(expectedIPBaseNumber, imn.IpBaseNumber);
        }

    }
}
