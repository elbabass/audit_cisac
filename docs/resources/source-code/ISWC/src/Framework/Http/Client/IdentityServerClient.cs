using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Http.Client
{
    public interface IRestApiIdentityServerClient
    {
        Task<string> RequestClientCredentialsTokenAsync(IDictionary<string, string> claims);
    }

    public interface IMatchingEngineIdentityServerClient
    {
        Task<string> RequestClientCredentialsTokenAsync(IDictionary<string, string> claims);
    }

    [ExcludeFromCodeCoverage]
    public class IdentityServerClient<TOptions> : IRestApiIdentityServerClient, IMatchingEngineIdentityServerClient where TOptions : ClientCredentialsOptions, new()
    {
        private readonly HttpClient httpClient;
        private readonly ClientCredentialsTokenRequest tokenRequest;

        public IdentityServerClient(
            HttpClient httpClient, IOptions<TOptions> options)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = options.Value.Address,
                ClientId = options.Value.ClientId,
                ClientSecret = options.Value.ClientSecret,
                Scope = options.Value.Scope
            };
        }

        public async Task<string> RequestClientCredentialsTokenAsync(IDictionary<string, string> claims)
        {
            tokenRequest.Parameters = claims;

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);
            if (tokenResponse.IsError)
            {
                throw new HttpRequestException($"Something went wrong while requesting the access token: {tokenResponse.Error}");
            }
            return tokenResponse.AccessToken;
        }
    }
}
