using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Bdo.Portal;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Portal.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserManger userManger;
        private readonly IHttpContextAccessor contextAccessor;

        public ProfileController(IUserManger userManger, IHttpContextAccessor contextAccessor)
        {
            this.userManger = userManger;
            this.contextAccessor = contextAccessor;
        }

        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<WebUser>> GetUserProfile([FromBody] WebUser webUser)
        {
            var authUser = GetUserClaims();
            if (authUser.IsMangeRoles)
                return Ok(await userManger.GetUserProfile(webUser));

            return Unauthorized();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<WebUserRole>>> GetUserRoles()
        {
            var authUser = GetUserClaims();
            return Ok(await userManger.GetUserRoles(authUser.UserId, authUser.AgencyId));
        }

        [HttpGet, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult<IEnumerable<WebUser>>> GetAllUsers()
        {
            var authUser = GetUserClaims();
            if (authUser.IsMangeRoles)
                return Ok(await userManger.GetAllUsers(authUser.UserId, authUser.AgencyId));

            return Unauthorized();
        }

        [HttpPost, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult> RequestAccess([FromBody] WebUserRole requestedRole)
        {
            var authUser = GetUserClaims();
            await userManger.RequestAccess(authUser.UserId, authUser.AgencyId, requestedRole);

            return Ok();
        }

        [HttpPut, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult> UpdateUserRole([FromBody] WebUser webUser)
        {
            var authUser = GetUserClaims();
            if (authUser.IsMangeRoles)
            {
                await userManger.UpdateUserRole(authUser.UserId, webUser);

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet, Authorize(AuthenticationSchemes = "Cookies")]
        public async Task<ActionResult> GetAccessRequests()
        {
            var authUser = GetUserClaims();
            if (authUser.IsMangeRoles)
                return Ok(await userManger.GetAccessRequests(authUser.UserId, authUser.AgencyId));

            return Unauthorized();
        }

        private AuthorizedUser GetUserClaims()
        {
            var userId = contextAccessor.HttpContext.User?.FindFirst("userId");
            var agencyId = contextAccessor.HttpContext.User?.FindFirst("agencyId");
            var isManageRole = contextAccessor.HttpContext.User?.FindFirst("isManageRoles");

            return new AuthorizedUser(userId?.Value, agencyId?.Value, isManageRole.Value);
        }
    }
}
