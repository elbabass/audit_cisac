using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class STN : BaseRecord
    {
        public STN(string record) : base(record) 
        { 

        }

        public string IPBaseNumber => record.GetField<string>(20, 13);
        public IpStatus StatusCode => record.GetField<IpStatus>(33, 1);
        public DateTime? ValidFrom => record.GetField<DateTime?>(34, 14, GetDateTimeFormat());
        public DateTime? ValidTo => record.GetField<DateTime?>(48, 14, GetDateTimeFormat());
        public DateTime? AmendmentDate => record.GetField<DateTime?>(62, 14, GetDateTimeFormat());
    }
}
