using IdentityModel.Client;
using LazyCache;
using SpanishPoint.Azure.Iswc.Framework.Http.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Http.Handler
{
    [ExcludeFromCodeCoverage]
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IMatchingEngineIdentityServerClient identityServerClient;
        private readonly IAppCache appCache;

        public ProtectedApiBearerTokenHandler(
            IMatchingEngineIdentityServerClient identityServerClient,
            IAppCache appCache)
        {
            this.identityServerClient = identityServerClient ?? throw new ArgumentNullException(nameof(identityServerClient));
            this.appCache = appCache ?? throw new ArgumentNullException(nameof(appCache));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var accessToken = await appCache.GetOrAddAsync("matching_api_access_token", async opt =>
            {
                opt.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(59);
                return await identityServerClient.RequestClientCredentialsTokenAsync(new Dictionary<string, string>());
            });

            request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
