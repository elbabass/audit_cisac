using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models;
using System;
using Xunit;

namespace SpanishPoint.Azure.Iswc.Data.Tests.Services.IpiService.Models
{
    /// <summary>
    /// IPA Deserializer - Test TLS Deserializer
    /// </summary>
    public class TLSTest
    {
        /// <summary>
        /// TLS Deserializer - Test the TLS record type deserializes correctly from string input
        /// </summary>
        [Fact]
        public void Deserialize_TLS_Record()
        {
            // Arrange
            var tlsRecord = "TLS000000010000000020200207135157035GEMA                NPAI-002522430-8        ";
            
            var expectedIpBaseNumber = "I-002522430-8";
            DateTime expectedTransactionDate = DateTime.ParseExact("20200207135157", "yyyyMMddHHmmss", null);

            // Act
            var tls = new TLS(tlsRecord);

            // Assert
            Assert.Equal(expectedIpBaseNumber, tls.IpBaseNumber);
            Assert.Equal(expectedTransactionDate, tls.TransactionDate);
        }
    }
}
