using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> Range(this DateTime startDate, DateTime endDate)
        {
            return Enumerable
                .Range(0, (int)(endDate - startDate).TotalDays + 1)
                .Select(i => startDate.AddDays(i));
        }
    }
}
