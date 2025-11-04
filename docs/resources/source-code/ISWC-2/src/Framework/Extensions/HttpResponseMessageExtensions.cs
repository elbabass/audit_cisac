using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<HttpResponseMessage> EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (response.Content != null)
                response.Content.Dispose();

            throw new SimpleHttpResponseException(response.StatusCode, content);
        }
    }

    public class SimpleHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public SimpleHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }

        public SimpleHttpResponseException()
        {
        }

        public SimpleHttpResponseException(string message) : base(message)
        {
        }

        public SimpleHttpResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
