using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class MCN : BaseRecord
    {
        public MCN(string record) : base(record)
        {

        }

        public long IpNameNumber => record.GetField<long>(20, 11);
        public string Name => record.GetField<string>(31, 90);
        public string NameType => record.GetField<string>(121, 2);
        public DateTime CreationDateTime => record.GetField<DateTime>(123, 14, GetDateTimeFormat());
        public DateTime AmendmentDateTime => record.GetField<DateTime>(137, 14, GetDateTimeFormat());
        public string IpBaseNumber => record.GetField<string>(151, 13);

    }
}
