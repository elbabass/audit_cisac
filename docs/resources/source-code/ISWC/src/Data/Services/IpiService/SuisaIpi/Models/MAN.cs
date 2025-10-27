using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;
using System.Globalization;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class MAN : BaseRecord
    {
        public MAN(string record) : base(record)
        {

        }

        public string SociteyCode => record.GetField<string>(20, 3);
        public string ClassCode => record.GetField<string>(23, 2);
        public string RoleCode => record.GetField<string>(25, 2);
        public string RightCode => record.GetField<string>(27, 2);
        public DateTime ValidFromDateTime => record.GetField<DateTime>(29, 14, GetDateTimeFormat());
        public DateTime ValidToDateTime => record.GetField<DateTime>(43, 14, GetDateTimeFormat());
        public DateTime DateOfSignature => record.GetField<DateTime>(57, 8, GetDateFormat());
        public decimal MembershipShare => GetSharePercentage(record.GetField<string>(65, 5));
        public DateTime AmendmentDateTime => record.GetField<DateTime>(70, 14, GetDateTimeFormat());

        decimal GetSharePercentage(string value)
        {
            if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal output))
                return 100.00m;

            return (output / 100);
        }

    }
}
