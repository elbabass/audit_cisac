namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToAgencyCode(this int sourceDb) => sourceDb.ToString().PadLeft(3, '0');
    }
}
