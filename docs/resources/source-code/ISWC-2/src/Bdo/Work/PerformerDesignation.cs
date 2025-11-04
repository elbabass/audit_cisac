using System.Runtime.Serialization;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public enum PerformerDesignation
    {
        [EnumMember(Value = @"Main Artist")]
        Main_Artist = 1,

        [EnumMember(Value = @"Other")]
        Other = 2,
    }
}
