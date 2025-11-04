using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class IMN : BaseRecord
    {
        public IMN(string record) : base(record)
        {

        }

        public long IpNameNumber => record.GetField<long>(20, 11);
        public string CreationCode => record.GetField<string>(31, 2);
        public string RoleCode => record.GetField<string>(33, 2);
        public string IpBaseNumber => record.GetField<string>(35, 13);
    }
}
