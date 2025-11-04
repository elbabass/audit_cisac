using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class TLS : BaseRecord
    {
        public TLS(string record) : base(record) 
        { 
        
        }

        public DateTime TransactionDate => record.GetField<DateTime>(20, 14, GetDateTimeFormat());
        public string RemittingSocietyCode => record.GetField<string>(34, 3);
        public string TransactionType => record.GetField<string>(37, 3);
        public string IpBaseNumber => record.GetField<string>(60, 13);
        public string Username => record.GetField<string>(53, 4);
    }
}
