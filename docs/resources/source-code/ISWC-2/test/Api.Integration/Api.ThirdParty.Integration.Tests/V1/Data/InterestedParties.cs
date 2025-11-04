using System.Collections.Generic;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Integration.Tests.V1.Data
{
    /// <summary>
    /// Interested Parties for specified agencies. Submissions will take the first two and the third can be used
    /// in update requests to add another eligible IP.
    /// </summary>
    public static class InterestedParties
    {
        public static List<InterestedParty> IP_AEPI => new List<InterestedParty>()
        {
            new InterestedParty() { Name = "KITSAKIS ALEXIOS", NameNumber = 16305322, Role = InterestedPartyRole.C },
            new InterestedParty() { Role = InterestedPartyRole.C, NameNumber = 16329302, Name = "KLAVAS KONSTANTINOS" },
            new InterestedParty() { Name = "KIRIAKAKIS SIMOS SIMEON", NameNumber = 16922590, Role = InterestedPartyRole.C },
            new InterestedParty() { Role = InterestedPartyRole.C, NameNumber = 255096361, Name = "NIKOLAOS KANAKIS" }
        };

        public static List<InterestedParty> IP_IMRO => new List<InterestedParty>()
        {
            new InterestedParty() { Name = "BOURKE CIARAN FRANCIS", NameNumber = 36303314, Role = InterestedPartyRole.C, LastName = "BOURKE"},
            new InterestedParty() { Name = "O BRIEN LIAM PATRICK", NameNumber = 159837032, Role = InterestedPartyRole.C, LastName = "O BRIEN"},
            new InterestedParty() { Name = "EDWARD JOHN SCHWEPPE", NameNumber = 46932859, Role = InterestedPartyRole.C, LastName = "EDWARD"}
        };

        public static List<InterestedParty> IP_AKKA => new List<InterestedParty>()
        {
            new InterestedParty() { Role = InterestedPartyRole.C, NameNumber = 254535957, Name = "KARKLINS ANDRIS" },
            new InterestedParty() { Role = InterestedPartyRole.C, NameNumber = 254522872, Name = "DAUDZVARDS AIGARS" },
            new InterestedParty() { Role = InterestedPartyRole.C, NameNumber = 367676606, Name = "BOITMANE AGNESE" }
        };

        public static List<InterestedParty> IP_PRS => new List<InterestedParty>()
        {
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 46893543, Name = "PARKS DAVID OCTAVIUS" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 46910187, Name = "BOOTHE DOUGLAS" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 184739236, Name = "ROME" }
       };

        public static List<InterestedParty> IP_SACEM => new List<InterestedParty>()
        {
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 44073, Name = "ABDEL WAHAB MOHAMED IBRAHIM EL SAFI" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 1738808, Name = "BABUSIAUX JULIE DESIREE MARIE" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 3811918, Name = "BOSSO GIUSEPPE ERNESTO" }
       };

        public static List<InterestedParty> IP_BMI => new List<InterestedParty>()
        {
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 1856409, Name = "BAKER BILL" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 1906032, Name = "REEVES KEN" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 1918414, Name = "FORSYTH WILL" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 296613240, Name = "EVANS TODD MARK", LastName = "EVANS"}
       };

        public static List<InterestedParty> IP_ASCAP => new List<InterestedParty>()
        {
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 18981263, Name = "MC SHANN JAY" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 19051605, Name = "DE MADINA FRANCISCO" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 18352103, Name = "LOGAN VIRGINIA KNIGHT" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 55285080, Name = "EVANS THOMAS", LastName = "EVANS"}
       };

        public static List<InterestedParty> IP_AMAR => new List<InterestedParty>()
        {
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 284685225, Name = "SILVA E LIMA JOSE AIRTON" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 67060094, Name = "JUSTO HELIO" },
            new InterestedParty() { Role = InterestedPartyRole.CA, NameNumber = 3253828, Name = "BITTENCOURT JACOB PICK" }
       };
    }
}
