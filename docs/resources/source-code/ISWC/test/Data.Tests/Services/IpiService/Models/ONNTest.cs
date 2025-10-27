using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// MCN Deserializer - Test MCN Deserializer
    /// </summary>
    public class ONNTest
    {
        /// <summary>
        /// IMN Deserializer - Test the IMN record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_ONN_Record()
        {
            // Arrange
            string onnRecord = "ONN000000350000000600466171448MENEZES                                                                                   ALEXANDRE MONTE DE                           DF200411221508002004112215080000254255376";
            
            long expectedIpNameNumber = 00466171448;
            var expectedName = "MENEZES";
            var expectedFirstName = "ALEXANDRE MONTE DE";
            var expectedNameType = "DF";
            DateTime? expectedCreationDate = DateTime.ParseExact("20041122150800", "yyyyMMddHHmmss", null);
            DateTime? expectedAmendedDate = DateTime.ParseExact("20041122150800", "yyyyMMddHHmmss", null);
            var expectedIPNumberRef = 00254255376;

            // Act
            var onn = new ONN(onnRecord);

            // Assert
            Assert.Equal(expectedIpNameNumber, onn.IpNameNumber);
            Assert.Equal(expectedName, onn.Name);
            Assert.Equal(expectedFirstName, onn.FirstName);
            Assert.Equal(expectedNameType, onn.NameType);
            Assert.Equal(expectedCreationDate, onn.CreationDateTime);
            Assert.Equal(expectedAmendedDate, onn.AmendmentDateTime);
            Assert.Equal(expectedIPNumberRef, onn.IpNameNumberRef);
        }
    }
}
