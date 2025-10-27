using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Security
{
    [ExcludeFromCodeCoverage]
    public class RequestFilterMiddleware
    {
        private readonly IConfiguration config;
        private readonly RequestDelegate next;
        private const string parameterNamePublicPortal = "BaseAddress-IswcPublic";
        private const string parameterNamePrivatePortal = "BaseAddress-IswcPrivate";
        private readonly IEnumerable<string> excludedPaths = new string[] { @"/api/health", @"/", @"/.well-known/openid-configuration", @"/connect/token" };

        public RequestFilterMiddleware(IConfiguration config, RequestDelegate next)
        {
            this.config = config;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            bool isPublicRequest = false;

            context.Request.Headers.TryGetValue("origin", out var origin);
            context.Request.Headers.TryGetValue("Request-Source", out var requestSource);
            context.Request.Headers.TryGetValue("Agent-Version", out var agentVersion);
            context.Request.Headers.TryGetValue("Public-Portal", out var publicPortal);

            if (config["ASPNETCORE_ENVIRONMENT"] != "Development" && !excludedPaths.Contains(context.Request.Path.Value))
            {
                context.Request.Headers.TryGetValue("Authorization", out var token);

                if (!IsBearerTokenValid(token))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden");

                    return;
                }
            }

            if (bool.TryParse(publicPortal, out bool isPublicPortal))
            {
                isPublicRequest = isPublicPortal;
            }

            if (!string.IsNullOrWhiteSpace(requestSource))
                requestSource = requestSource.FirstOrDefault().ToUpper();

            if (string.IsNullOrWhiteSpace(requestSource))
                requestSource = string.Empty;

            if (string.IsNullOrWhiteSpace(agentVersion))
                agentVersion = string.Empty;

            context.Items.Add("isPublicRequest", isPublicRequest);
            context.Items.Add("requestSource", requestSource);
            context.Items.Add("agentVersion", agentVersion);

            await next.Invoke(context);
        }

        private bool IsBearerTokenValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            string tokenValue = token.Replace("Bearer", string.Empty).Trim();

            var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);

            var clientId = parsedToken.Claims.FirstOrDefault(x => x.Type == "client_id");

            if (clientId?.Value.ToString() != "iswcapimanagement")
                return false;

            return true;
        }
    }
}
