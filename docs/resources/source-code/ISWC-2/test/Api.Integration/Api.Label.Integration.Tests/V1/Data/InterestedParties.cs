using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.V1.Data
{
    public class InterestedParties
    {
        public static List<InterestedParty> IP => new List<InterestedParty>()
        {
            new InterestedParty() { Name = "FREDDIE", LastName = "MERCURY", NameNumber = 00077406269, Role = InterestedPartyRole.CA }
        };

        public static List<InterestedParty> IP_SESAC => new List<InterestedParty>()
        {
            new InterestedParty() { Name = "Hy", LastName = "Zaret", Role = InterestedPartyRole.CA },
            new InterestedParty() { Name = "Alex", LastName = "North", Role = InterestedPartyRole.CA }
        };

        public static List<InterestedParty> IP_CISAC => new List<InterestedParty>()
        {
            new InterestedParty() { Name = "Andrew", LastName = "Hozier Byrne", DisplayName = "Hozier", Role = InterestedPartyRole.CA }
        };
    }
}
