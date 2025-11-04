using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test INN Deserializer
    /// </summary>
    public class INNTest
    {
        /// <summary>
        /// INN Deserializer - Test the INN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_INN_Record_NullDeathDate()
        {
            // Arrange
            string innRecord = "INN000000350000000700466171448MWLY                                                                                                                                                                            ";

            long expectedIpNameNumber = 00466171448;
            var expectedCreationClass = "MW";
            var expectedRole = "LY";


            // Act
            var inn = new INN(innRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, inn.IpNameNumber);
            Assert.Equal(expectedCreationClass, inn.CreationCode);
            Assert.Equal(expectedRole, inn.RoleCode);
        }

    }
}
