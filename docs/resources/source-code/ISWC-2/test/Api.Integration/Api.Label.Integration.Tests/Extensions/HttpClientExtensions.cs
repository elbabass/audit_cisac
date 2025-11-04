using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Label.Integration.Tests.Extensions
{
    internal static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string uri)
        {
            using HttpResponseMessage response = (await client.GetAsync(uri)).EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(res, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
