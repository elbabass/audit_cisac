using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test IMN Deserializer
    /// </summary>
    public class MANTest
    {
        /// <summary>
        /// IMN Deserializer - Test the IMN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_MAN_Record()
        {
            // Arrange
            var manRecord = "MAN0000000000000015129MWLYRB2019123000000099991231235959201912301000020200125000827";

            var expectedSocietyCode = "129";
            var expectedClassCode = "MW";
            var expectedRoleCode = "LY";
            var expectedRightCode = "RB";
            DateTime? expectedValidFrom = DateTime.ParseExact("20191230000000", "yyyyMMddHHmmss", null);
            DateTime? expectedValidTo = DateTime.ParseExact("99991231235959", "yyyyMMddHHmmss", null);
            DateTime? expectedDateOfSignature = DateTime.ParseExact("20191230", "yyyyMMdd", null);
            decimal? expectedMembershipShare = 100.00m;
            DateTime? expectedAmendmentDate = DateTime.ParseExact("20200125000827", "yyyyMMddHHmmss", null);

            // Act
            var man = new MAN(manRecord);

            // Assert
            Assert.Equal(expectedSocietyCode, man.SociteyCode);
            Assert.Equal(expectedClassCode, man.ClassCode);
            Assert.Equal(expectedRoleCode, man.RoleCode);
            Assert.Equal(expectedRightCode, man.RightCode);
            Assert.Equal(expectedValidFrom, man.ValidFromDateTime);
            Assert.Equal(expectedValidTo, man.ValidToDateTime);
            Assert.Equal(expectedDateOfSignature, man.DateOfSignature);
            Assert.Equal(expectedMembershipShare, man.MembershipShare);
            Assert.Equal(expectedAmendmentDate, man.AmendmentDateTime);
        }

    }
}
