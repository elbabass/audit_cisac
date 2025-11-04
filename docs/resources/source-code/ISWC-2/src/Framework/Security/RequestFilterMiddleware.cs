using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
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

        private static bool TryGetHeaderValue(IHeaderDictionary headers, string headerName, out string value)
        {
            foreach (var kvp in headers)
            {
                if (string.Equals(kvp.Key, headerName, StringComparison.OrdinalIgnoreCase))
                {
                    value = kvp.Value.FirstOrDefault() ?? string.Empty;
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }

        public async Task Invoke(HttpContext context)
        {
            bool isPublicRequest = false;

            var headers = context.Request.Headers;
            TryGetHeaderValue(headers, "origin", out var origin);
            TryGetHeaderValue(headers, "request-source", out var requestSource);
            TryGetHeaderValue(headers, "agent-version", out var agentVersion);
            TryGetHeaderValue(headers, "public-portal", out var publicPortal);

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
                requestSource = requestSource?.Trim().ToUpperInvariant() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(requestSource))
                requestSource = string.Empty;

            if (string.IsNullOrWhiteSpace(agentVersion))
                agentVersion = string.Empty;

            context.Items["isPublicRequest"] = isPublicRequest;
            context.Items["requestSource"] = requestSource;
            context.Items["agentVersion"] = agentVersion;

            await next.Invoke(context);
        }

        private bool IsBearerTokenValid(string? token)
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
