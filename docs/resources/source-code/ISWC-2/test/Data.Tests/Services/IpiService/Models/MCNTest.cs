using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// MCN Deserializer - Test MCN Deserializer
    /// </summary>
    public class MCNTest
    {
        /// <summary>
        /// IMN Deserializer - Test the IMN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_MCN_Record()
        {
            // Arrange
            string mcnRecord = "MCN000001240000002100276255644NITO JUAREZ                                                                               PG2000060100020520000602032906I-000000125-0                                           ";
            
            long expectedIpNameNumber = 00276255644;
            string expectedName = "NITO JUAREZ";
            string expectedNameType = "PG";
            DateTime? expectedCreationDate = DateTime.ParseExact("20000601000205", "yyyyMMddHHmmss", null);
            DateTime? expectedAmendedDate = DateTime.ParseExact("20000602032906", "yyyyMMddHHmmss", null);
            string expectedIPBaseNumber = "I-000000125-0";

            // Act
            var mcn = new MCN(mcnRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, mcn.IpNameNumber);
            Assert.Equal(expectedName, mcn.Name);
            Assert.Equal(expectedNameType, mcn.NameType);
            Assert.Equal(expectedCreationDate, mcn.CreationDateTime);
            Assert.Equal(expectedAmendedDate, mcn.AmendmentDateTime);
            Assert.Equal(expectedIPBaseNumber, mcn.IpBaseNumber);
        }
    }
}
