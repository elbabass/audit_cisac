using System;
using Xunit;
using SpanishPoint.Azure.Iswc.Framework.Extensions;

namespace SpanishPoint.Azure.Iswc.Framework.Tests.Extensions
{
    /// <summary>
    /// Tests DateTimeExtensions
    /// </summary>
    public class DateTimeExtensionsTest
    {
        /// <summary>
        /// Test succeeds if value is returned as expected
        /// </summary>
        [Fact]
        public void DateTime_returned()
        {
            var startDate = DateTime.Parse("2000-12-30 00:00:00");
            var endDate = DateTime.Parse("2000-12-31 00:00:00");
            var value = DateTimeExtensions.Range(startDate, endDate);
            Assert.NotNull(value);
            Assert.Collection(value, 
                date1 => Assert.Equal(startDate, date1),
                date2 => Assert.Equal(endDate, date2));
        }
    }
}
