using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test NCN Deserializer
    /// </summary>
    public class MUNTest
    {
        /// <summary>
        /// NCN Deserializer - Test the NCN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_MUN_Record()
        {
            // Arrange
            var munRecord = "MUN000001240000002200276255644DMMCI-000000125-0                                                                                                                                                               ";

            var expectedIpNameNumber = 00276255644;
            var expectedCreationClass = "DM";
            var expectedRole = "MC";
            var expectedBaseNumber = "I-000000125-0";
                
            // Act
            var mun = new MUN(munRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, mun.IpNameNumber);
            Assert.Equal(expectedCreationClass, mun.CreationCode);
            Assert.Equal(expectedRole, mun.RoleCode);
            Assert.Equal(expectedBaseNumber, mun.IpBaseNumber);
        }
    }
}


