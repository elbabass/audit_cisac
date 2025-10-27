using System.Security.Cryptography;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class HashExtensions
    {
        public static string GetHashValue(this string dataToHash)
        {
            if (string.IsNullOrWhiteSpace(dataToHash)) return string.Empty;

            var data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            var result = new StringBuilder();

            foreach (var @byte in data)
            {
                //x2 converts the byte to a hexadecimal string
                result.Append(@byte.ToString("x2"));
            }

            return result.ToString();
        }
    }
}
