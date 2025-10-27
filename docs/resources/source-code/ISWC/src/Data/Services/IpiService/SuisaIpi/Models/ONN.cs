using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class ONN : BaseRecord
    {
        public ONN(string record) : base(record)
        {

        }

        public long IpNameNumber => record.GetField<long>(20, 11);
        public string Name => record.GetField<string>(31, 90);
        public string FirstName => record.GetField<string>(121, 45);
        public string NameType => record.GetField<string>(166, 2);
        public DateTime CreationDateTime => record.GetField<DateTime>(168, 14, GetDateTimeFormat());
        public DateTime AmendmentDateTime => record.GetField<DateTime>(182, 14, GetDateTimeFormat());
        public long? IpNameNumberRef => record.GetField<long?>(196, 11);
    }
}
