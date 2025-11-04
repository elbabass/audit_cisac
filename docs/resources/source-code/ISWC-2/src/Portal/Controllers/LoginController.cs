using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Client;
using SpanishPoint.Azure.Iswc.Portal.Configuration.Options;
using SpanishPoint.Azure.Iswc.Portal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SpanishPoint.Azure.Iswc.Portal.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly IFastTrackAuthenticationService fastTrackAuthenticationService;
        private readonly IOptions<ClientAppOptions> options;
        private readonly IRestApiIdentityServerClient identityServerClient;
        private readonly ILogger<LoginController> logger;
        private readonly IUserManger userManger;

        public LoginController(
            IFastTrackAuthenticationService fastTrackAuthenticationService,
            IOptions<ClientAppOptions> options,
            IRestApiIdentityServerClient identityServerClient,
            ILogger<LoginController> logger,
            IUserManger userManger)
        {
            this.fastTrackAuthenticationService = fastTrackAuthenticationService;
            this.options = options;
            this.identityServerClient = identityServerClient;
            this.logger = logger;
            this.userManger = userManger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string userid, [FromForm] string passport, [FromForm] string redirecturl)
        {
            var path = redirecturl ?? "/search";
            var builder = new UriBuilder($"http://example.com{path}");
            var userDetails = await fastTrackAuthenticationService.AuthenticateUser(new UserAutheticationModel()
            {
                UserId = userid,
                Passport = passport
            });

            if (string.IsNullOrEmpty(userDetails?.AgentID))
            {
                logger.LogError("AgentID not provided from FastTrack Authentication Service.");
                Redirect(options.Value.LoginRedirectUri.AbsoluteUri);
            }

            var query = HttpUtility.ParseQueryString(builder.Uri.Query);

            var user = await userManger.AddUserProfile(userDetails.Email, userDetails.AgentID);
            userDetails.Roles = StringExtensions.CreateSemiColonSeperatedString(user.WebUserRoles.Select(x => ((int)x.Role).ToString()));

            var isManageRoles = user.WebUserRoles.Any(x => x.IsApproved && x.Role == Bdo.Portal.PortalRoleType.ManageRoles);

            query["token_id"] = await identityServerClient.RequestClientCredentialsTokenAsync(userDetails.AsDictionary());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim("userId", user.Email),
                new Claim("agencyId", user.AgencyId),
                new Claim("isManageRoles", isManageRoles.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            builder.Query = query.ToString();

            return Redirect(builder.Uri.PathAndQuery);
        }

        [HttpPost("/login/localdevelopment")]
        public async Task<IActionResult> LocalDevelopment([FromForm] string userid, [FromForm] string password, [FromForm] string redirecturl)
        {
            var passport = await fastTrackAuthenticationService.LoginUser(new UserAutheticationModel { UserId = userid, Password = password });

            return await Post(userid, passport, redirecturl);
        }
    }
}
