using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    abstract class BaseRecord
    {
        internal readonly string record;

        public BaseRecord(string record)
        {
            this.record = record;
        }

        internal enum DateType
        {
            Date,
            DateTime
        }

        public string RecordType => record.GetField<string>(1, 3);
        public string TransactionSequenceNo => record.GetField<string>(4, 8);
        public string DetailRecordSequenceNo => record.GetField<string>(12, 8);

        public string GetDateTimeFormat() => "yyyyMMddHHmmss";
        public string GetDateFormat() => "yyyyMMdd";


        internal DateTime? BuildDateFromStrings(string year, string month, string day, string time, DateType dateType)
        {
            if (string.IsNullOrWhiteSpace(year) || year.Length < 4 || year == "0000")
                return null;

            if (string.IsNullOrWhiteSpace(month) || month.Length < 2)
                month = "01";

            if (string.IsNullOrWhiteSpace(day) || day.Length < 2)
                day = "01";

            if (dateType == DateType.Date)
                return DateTime.ParseExact($"{year}{month}{day}", GetDateFormat(), null);

            else
            {
                if (string.IsNullOrWhiteSpace(time) || time.Length < 6)
                    time = "000000";

                return DateTime.ParseExact($"{year}{month}{day}{time}", GetDateTimeFormat(), null);
            }

        }
    }
}
