using System;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test NCN Deserializer
    /// </summary>
    public class NCNTest
    {
        /// <summary>
        /// NCN Deserializer - Test the NCN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_NCN_Record()
        {
            // Arrange
            var ncnRecord = "NCN000000000000458710001832027MICHAEL MENGES MUSIKVERLAG/MICHAEL MENGES MUSIKMANAGEMENT E.K.                                                                         MO2019122900322620191229014224";

            var expectedIpNameNumber = 10001832027;
            DateTime expectedAmendmentDate = DateTime.ParseExact("20191229014224", "yyyyMMddHHmmss", null);
            string expectedFirstName = null;
            var expectedName = "MICHAEL MENGES MUSIKVERLAG/MICHAEL MENGES MUSIKMANAGEMENT E.K.";
            var expectedCreatedDate = DateTime.ParseExact("20191229003226", "yyyyMMddHHmmss", null);
            var expectedTypeCode = NameType.MO;
                
            // Act
            var ncn = new NCN(ncnRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, ncn.IPNameNumber);
            Assert.Equal(expectedAmendmentDate, ncn.AmendmentDate);
            Assert.Equal(expectedFirstName, ncn.FirstName);
            Assert.Equal(expectedName, ncn.Name);
            Assert.Equal(expectedCreatedDate, ncn.CreationDate);
            Assert.Equal(expectedTypeCode, ncn.NameType);
        }
    }
}


