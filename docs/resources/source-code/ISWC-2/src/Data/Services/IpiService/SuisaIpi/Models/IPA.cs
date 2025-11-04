using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class IPA : BaseRecord
    {
        public IPA(string record) : base(record) 
        { 
        
        }

        public DateTime? TransactionDate => record.GetField<DateTime?>(20, 14, GetDateTimeFormat());
        public string RemittingSocietyCode => record.GetField<string>(34, 3);
        public string RemittingSocietyName => record.GetField<string>(37, 20);
        public string IpBaseNumber => record.GetField<string>(57, 13);
        public int StatusCode => record.GetField<int>(70, 1);

        public long IpNameNumber => record.GetField<long>(71, 11);
        public string NameType => record.GetField<string>(82, 2);
        public string ReferenceIpBaseNumber => record.GetField<string>(84, 13);
        public int ReferenceStatusCode => record.GetField<int>(97, 1);
        public string PatronymIpBaseNumber => record.GetField<string>(98, 11);
        public string PatronymNameType => record.GetField<string>(109, 2);

    }
}
