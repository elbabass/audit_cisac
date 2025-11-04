using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class NCN : BaseRecord
    {
        public NCN(string record) : base(record) 
        { 
        
        }

        public long IPNameNumber => record.GetField<long>(20, 11);
        public string Name => record.GetField<string>(31, 90);
        public string FirstName => record.GetField<string>(121, 45);
        public NameType NameType => record.GetField<NameType>(166, 2);
        public DateTime CreationDate => record.GetField<DateTime>(168, 14, GetDateTimeFormat());
        public DateTime AmendmentDate => record.GetField<DateTime>(182, 14, GetDateTimeFormat());
    }

}
