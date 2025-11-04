using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.V1.Data
{
    /// <summary>
    /// Interested Parties for specified agencies. Submissions will take the first two and the third can be used
    /// in update requests to add another eligible IP.
    /// </summary>
    public static class InterestedParties
    {
        public static List<InterestedParty> IP_AEPI => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber="I-000243145-8", Name = "KITSAKIS ALEXIOS", NameNumber = 16305322, Role = InterestedPartyRole.C },
            new InterestedParty() { BaseNumber = "I-000213512-6", Role = InterestedPartyRole.C, NameNumber = 16329302, Name = "KLAVAS KONSTANTINOS" },
            new InterestedParty() { BaseNumber="I-000278714-4", Name = "KIRIAKAKIS SIMOS SIMEON", NameNumber = 16922590, Role = InterestedPartyRole.C },
            new InterestedParty() { BaseNumber = "I-000122272-4", Role = InterestedPartyRole.C, NameNumber = 255096361, Name = "NIKOLAOS KANAKIS" }
        };

        public static List<InterestedParty> IP_IMRO => new List<InterestedParty>()
        {
            new InterestedParty() { Name = "BOURKE CIARAN FRANCIS", NameNumber = 36303314, Role = InterestedPartyRole.C, BaseNumber = "I-000380434-8", LastName = "BOURKE"},
            new InterestedParty() { Name = "O BRIEN LIAM PATRICK", NameNumber = 159837032, Role = InterestedPartyRole.C, BaseNumber = "I-000139657-0", LastName = "O BRIEN"},
            new InterestedParty() { Name = "EDWARD JOHN SCHWEPPE", NameNumber = 46932859, Role = InterestedPartyRole.C, BaseNumber = "I-001221086-3", LastName = "EDWARD"}
        };

        public static List<InterestedParty> IP_AKKA => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber = "I-001626505-5", Role = InterestedPartyRole.C, NameNumber = 254535957, Name = "KARKLINS ANDRIS" },
            new InterestedParty() { BaseNumber = "I-000279494-5", Role = InterestedPartyRole.C, NameNumber = 254522872, Name = "DAUDZVARDS AIGARS" },
            new InterestedParty() { BaseNumber = "I-005078057-7", Role = InterestedPartyRole.C, NameNumber = 367676606, Name = "BOITMANE AGNESE" }
        };

        public static List<InterestedParty> IP_PRS => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber = "I-001248136-4", Role = InterestedPartyRole.CA, NameNumber = 46893543, Name = "PARKS DAVID OCTAVIUS", LastName = "PARKS" },
            new InterestedParty() { BaseNumber = "I-001236670-8", Role = InterestedPartyRole.CA, NameNumber = 46910187, Name = "BOOTHE DOUGLAS", LastName = "BOOTHE" },
            new InterestedParty() { BaseNumber = "I-001461911-3", Role = InterestedPartyRole.CA, NameNumber = 184739236, Name = "ROME" }
       };

        public static List<InterestedParty> IP_SACEM => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber = "I-000964757-6", Role = InterestedPartyRole.CA, NameNumber = 44073, Name = "ABDEL WAHAB MOHAMED IBRAHIM EL SAFI" },
            new InterestedParty() { BaseNumber = "I-001077352-9", Role = InterestedPartyRole.CA, NameNumber = 1738808, Name = "BABUSIAUX JULIE DESIREE MARIE" },
            new InterestedParty() { BaseNumber = "I-000985921-4", Role = InterestedPartyRole.CA, NameNumber = 3811918, Name = "BOSSO GIUSEPPE ERNESTO" }
       };

        public static List<InterestedParty> IP_BMI => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber = "I-000938234-5", Role = InterestedPartyRole.CA, NameNumber = 1856409, Name = "BAKER BILL", LastName = "BAKER" },
            new InterestedParty() { BaseNumber = "I-000682468-4", Role = InterestedPartyRole.CA, NameNumber = 1906032, Name = "REEVES KEN", LastName = "REEVES" },
            new InterestedParty() { BaseNumber = "I-000753543-3", Role = InterestedPartyRole.CA, NameNumber = 1918414, Name = "FORSYTH WILL", LastName = "FORSYTH" },
            new InterestedParty() { BaseNumber = "I-001744474-3", Role = InterestedPartyRole.CA, NameNumber = 296613240, Name = "EVANS TODD MARK", LastName = "EVANS"}
       };

        public static List<InterestedParty> IP_ASCAP => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber = "I-000634187-1", Role = InterestedPartyRole.CA, NameNumber = 18981263, Name = "MC SHANN JAY" },
            new InterestedParty() { BaseNumber = "I-000667960-1", Role = InterestedPartyRole.CA, NameNumber = 19051605, Name = "DE MADINA FRANCISCO" },
            new InterestedParty() { BaseNumber = "I-000590757-1", Role = InterestedPartyRole.CA, NameNumber = 18352103, Name = "LOGAN VIRGINIA KNIGHT" },
            new InterestedParty() { BaseNumber = "I-000586180-1", Role = InterestedPartyRole.CA, NameNumber = 55285080, Name = "EVANS THOMAS", LastName = "EVANS"}
       };

        public static List<InterestedParty> IP_AMAR => new List<InterestedParty>()
        {
            new InterestedParty() { BaseNumber = "I-000001369-2", Role = InterestedPartyRole.CA, NameNumber = 284685225, Name = "SILVA E LIMA JOSE AIRTON" },
            new InterestedParty() { BaseNumber = "I-000043979-0", Role = InterestedPartyRole.CA, NameNumber = 67060094, Name = "JUSTO HELIO" },
            new InterestedParty() { BaseNumber = "I-000037777-3", Role = InterestedPartyRole.CA, NameNumber = 3253828, Name = "BITTENCOURT JACOB PICK" }
       };
    }
}
