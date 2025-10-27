using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Authentication.IdentityServer
{
    [ExcludeFromCodeCoverage]
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("iswcapi", "ISWC API")
            };

        public static IEnumerable<Client> Clients(IConfiguration configuration) =>
            new Client[]
            {
                new Client
                {
                    ClientId = "iswcportal",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret(configuration["Secret-IswcApi"].Sha256()) },
                    AllowedScopes = { "iswcapi" }
                },
                new Client
                {
                    ClientId = "iswcapimanagement",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret(configuration["Secret-IswcApiManagement"].Sha256()) },
                    AllowedScopes = { "iswcapi" }
                }
            };
    }
}
