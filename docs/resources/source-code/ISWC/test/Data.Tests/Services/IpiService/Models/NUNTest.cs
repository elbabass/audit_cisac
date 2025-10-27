using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test NCN Deserializer
    /// </summary>
    public class NUNTest
    {
        /// <summary>
        /// NCN Deserializer - Test the NCN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_NUN_Record()
        {
            // Arrange
            var nunRecord = "NUN000000000000000401017040713MWLY";

            var expectedIpNameNumber = 01017040713;
            var expectedCreationClass = "MW";
            var expectedRole = "LY";
                
            // Act
            var nun = new NUN(nunRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, nun.IpNameNumber);
            Assert.Equal(expectedCreationClass, nun.CreationCode);
            Assert.Equal(expectedRole, nun.RoleCode);
        }
    }
}


