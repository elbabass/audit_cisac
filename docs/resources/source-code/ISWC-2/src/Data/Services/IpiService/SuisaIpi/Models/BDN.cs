using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Extensions;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IpiService.SuisaIpi.Models
{
    internal class BDN : BaseRecord
    {
        public BDN(string record) : base(record)
        {

        }

        public InterestedPartyType InterestedPartyType => record.GetField<InterestedPartyType>(20, 1);
        public string YearOfBirth => record.GetField<string>(21, 4);
        public string MonthOfBirth => record.GetField<string>(25, 2);
        public string DayOfBirth => record.GetField<string>(27, 2);
        public string YearOfDeath => record.GetField<string>(29, 4);
        public string MonthOfDeath => record.GetField<string>(33, 2);
        public string DayOfDeath => record.GetField<string>(35, 2);
        public string PlaceOfBirth => record.GetField<string>(37, 30);
        public string StateOfBirth => record.GetField<string>(67, 30);
        public string TISNBirthCountry => record.GetField<string>(97, 4);
        public DateTime? TISNValidFrom => record.GetField<DateTime?>(101, 8, GetDateFormat());
        public string TISANBirthCountry => record.GetField<string>(109, 20);
        public DateTime? TISANValidFrom => record.GetField<DateTime?>(129, 8, GetDateFormat());
        public string Sex => record.GetField<string>(137, 1);
        public DateTime? AmendedDate => record.GetField<DateTime?>(138, 14, GetDateTimeFormat());

        public DateTime? GetDateOfBirth() => BuildDateFromStrings(YearOfBirth, MonthOfBirth, DayOfBirth, null, DateType.Date);
        public DateTime? GetDateOfDeath() => BuildDateFromStrings(YearOfDeath, MonthOfDeath, DayOfDeath, null, DateType.Date);
    }
}
