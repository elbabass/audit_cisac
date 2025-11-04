using IdentityServer4.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Authentication.IdentityServer
{
    [ExcludeFromCodeCoverage]
    public class AgentClaimsValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var agentid = context.Result.ValidatedRequest.Raw["AgentID"];
            var email = context.Result.ValidatedRequest.Raw["Email"];
            var roles = context.Result.ValidatedRequest.Raw["Roles"];

            if (string.IsNullOrWhiteSpace(agentid))
            {
                context.Result.Error = "AgentID claim was not provided.";
                context.Result.IsError = true;
            }
            else
            {
                context.Result.ValidatedRequest.Client.ClientClaimsPrefix = string.Empty;
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("agent_id", agentid));
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("email", email ?? string.Empty));
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("roles", roles ?? string.Empty));
                context.Result.ValidatedRequest.ClientClaims.Add(new Claim("access_scope", roles != null && roles.Contains("2") ? "update" : "search"));
            }

            return Task.FromResult(0);
        }
    }
}
