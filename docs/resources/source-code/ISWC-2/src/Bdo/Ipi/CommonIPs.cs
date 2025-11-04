using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Ipi
{
    public static class CommonIPs
    {
        public const string PublicDomain = "I-001635861-3";
        public const string DP = "I-001635861-3";
        public const string TRAD = "I-002296966-8";
        public const string UnknownComposerAuthor = "I-001635620-8";

        public static readonly IEnumerable<string> PublicDomainIps = new List<string> { PublicDomain, DP, TRAD, UnknownComposerAuthor };
    }
}
