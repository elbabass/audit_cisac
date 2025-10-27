using System;

namespace SpanishPoint.Azure.Iswc.Framework.Http.Client
{
    public abstract class ClientCredentialsOptions
    {
        public string? Address { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Scope { get; set; }
    }
}
