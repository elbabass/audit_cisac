using AutoMapper;
using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Bdo.Portal;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{

    public interface IUserManger
    {
        Task<WebUser> AddUserProfile(string userId, string agencyId);
        Task<WebUser> GetUserProfile(WebUser webUser);
        Task<IEnumerable<WebUserRole>> GetUserRoles(string userId, string agencyId);
        Task<IEnumerable<WebUser>> GetAllUsers(string userId, string agencyId);
        Task<IEnumerable<WebUser>> GetAccessRequests(string userId, string agencyId);
        Task RequestAccess(string userId, string agencyId, WebUserRole webUserRole);
        Task UpdateUserRole(string userId, WebUser webUser);
    }

    public class UserManager : IUserManger
    {
        private readonly IWebUserRepository webUserRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        private const string CisacRole = "312";

        public UserManager(IWebUserRepository webUserRepository, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            this.webUserRepository = webUserRepository;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<WebUser> AddUserProfile(string userId, string agencyId)
        {
            var unitOfWork = webUserRepository.UnitOfWork;
            var now = DateTime.UtcNow;

            if (!await webUserRepository.ExistsAsync(x => x.Email == userId && x.Status))
            {
                var newUser = new Data.DataModels.WebUser
                {
                    Email = userId,
                    AgencyId = agencyId,
                    CreatedDate = now,
                    LastModifiedDate = now,
                    Status = true
                };

                newUser.WebUserRole.Add(
                        new Data.DataModels.WebUserRole
                        {
                            IsApproved = true,
                            RoleId = (int)PortalRoleType.Search,
                            CreatedDate = now,
                            LastModifiedDate = now,
                            Status = true
                        }
                    );

                if (CheckAgencyIsCisac(agencyId))
                    newUser.WebUserRole.Add(
                        new Data.DataModels.WebUserRole
                        {
                            IsApproved = true,
                            RoleId = (int)PortalRoleType.ManageRoles,
                            CreatedDate = now,
                            LastModifiedDate = now,
                            Status = true
                        }
                    );

                await webUserRepository.AddAsync(newUser);
                await unitOfWork.Save();
            }

            var user = await GetUserProfile(new WebUser { Email = userId, AgencyId = agencyId });
            user.WebUserRoles = user.WebUserRoles.Where(x => x.IsApproved).ToList();

            return user;
        }

        public async Task<WebUser> GetUserProfile(WebUser webUser)
        {
            return mapper.Map<WebUser>(await webUserRepository.FindAsyncOptimizedByPath(x => x.Email == webUser.Email && x.AgencyId == webUser.AgencyId && x.Status
            && x.WebUserRole.Any(y => y.Status),
                $"{nameof(WebUserRole)}"));
        }


        public async Task<IEnumerable<WebUserRole>> GetUserRoles(string userId, string agencyId)
        {
            var user = mapper.Map<WebUser>(await webUserRepository.FindAsyncOptimizedByPath(x => x.Email == userId && x.AgencyId == agencyId && x.Status
            && x.WebUserRole.Any(y => y.Status),
                $"{nameof(WebUserRole)}"));

            return user.WebUserRoles;
        }

        public async Task RequestAccess(string userId, string agencyId, WebUserRole webUserRole)
        {
            var unitOfWork = webUserRepository.UnitOfWork;
            var now = DateTime.UtcNow;


            var user = await webUserRepository.FindAsyncOptimizedByPath(x => x.Email == userId && x.Status,
                 $"{nameof(WebUserRole)}");

            var requestedRole = new Data.DataModels.WebUserRole
            {
                IsApproved = false,
                CreatedDate = now,
                LastModifiedDate = now,
                RoleId = (int)webUserRole.Role,
                Status = true
            };

            requestedRole.Notification.Add(new Data.DataModels.Notification
            {
                CreatedDate = now,
                LastModifiedDate = now,
                Status = true,
                Message = webUserRole?.Notification?.Message,
                NotificationTypeId = (int)NotificationType.AccessRequest
            });

            user.WebUserRole.Add(requestedRole);

            await webUserRepository.UpdateAsync(user);
            await unitOfWork.Save();
        }


        public async Task<IEnumerable<WebUser>> GetAllUsers(string userId, string agencyId)
        {
            if (CheckAgencyIsCisac(agencyId))
                return mapper.Map<List<WebUser>>(await webUserRepository.FindManyAsyncOptimizedByPath(x => x.Status,
                    $"{nameof(WebUserRole)}"));

            return mapper.Map<List<WebUser>>(await webUserRepository.FindManyAsyncOptimizedByPath(x => x.Status && x.AgencyId == agencyId,
                    $"{nameof(WebUserRole)}"));

        }

        public async Task<IEnumerable<WebUser>> GetAccessRequests(string userId, string agencyId)
        {
            var users = new List<WebUser>();
            if (CheckAgencyIsCisac(agencyId))
                users = (mapper.Map<List<WebUser>>(await webUserRepository.FindManyAsyncOptimizedByPath(x => x.Status,
                  $"{nameof(WebUserRole)}", $"{nameof(WebUserRole)}.{nameof(Notification)}"))).Where(x => x.WebUserRoles.Any(x => !x.IsApproved)).ToList();
            else
                users = (mapper.Map<List<WebUser>>(await webUserRepository.FindManyAsyncOptimizedByPath(x => x.AgencyId == agencyId && x.Status,
                    $"{nameof(WebUserRole)}", $"{nameof(WebUserRole)}.{nameof(Notification)}"))).Where(x => x.WebUserRoles.Any(x => !x.IsApproved)).ToList();

            for (var x = 0; x < users.Count; x++)
            {
                var user = users.ElementAt(x);
                user.WebUserRoles = user.WebUserRoles.Where(x => !x.IsApproved).ToList();
            }

            return users.Where(x => x.WebUserRoles.Count > 0);
        }

        public async Task UpdateUserRole(string agencyId, WebUser webUser)
        {
            var unitOfWork = webUserRepository.UnitOfWork;
            var now = DateTime.UtcNow;

            var user = await webUserRepository.FindAsyncOptimizedByPath(x => x.Email == webUser.Email && x.Status,
                $"{nameof(WebUserRole)}", $"{nameof(WebUserRole)}.{nameof(Notification)}");

            var updatedRole = webUser.WebUserRoles.FirstOrDefault();

            if (updatedRole.Status == false && updatedRole.Role == PortalRoleType.ManageRoles
                && CheckAgencyIsCisac(user.AgencyId) && !await CisacHasOtherManageRole(webUser))
                return;

            if (user.WebUserRole.Any(x => webUser.WebUserRoles.Any(r => x.RoleId == (int)r.Role)))
            {
                user.WebUserRole.Where(x => webUser.WebUserRoles.Any(r => x.RoleId == (int)r.Role))
                    .Select(r =>
                    {
                        UpdateWebUserRole(r, updatedRole);
                        return r;
                    }).ToList();
            }
            else
            {
                user.WebUserRole.Add(new Data.DataModels.WebUserRole
                {
                    CreatedDate = now,
                    LastModifiedDate = now,
                    IsApproved = updatedRole.IsApproved,
                    Status = updatedRole.IsApproved && updatedRole.Status,
                    RoleId = (int)updatedRole.Role
                });
            }

            if (updatedRole.Role == PortalRoleType.ReportBasics && !updatedRole.IsApproved && !updatedRole.Status)
            {
                var webUserRoles = user.WebUserRole.Where(
                    x => x.RoleId == (int)PortalRoleType.ReportExtracts || x.RoleId == (int)PortalRoleType.ReportFullExtract
                    || x.RoleId == (int)PortalRoleType.ReportAgencyInterest).ToList();

                for (var x = 0; x < webUserRoles.Count; x++)
                    UpdateWebUserRole(webUserRoles.ElementAt(x), updatedRole);
            }

            await webUserRepository.UpdateAsync(user);
            await unitOfWork.Save();

            void UpdateWebUserRole(Data.DataModels.WebUserRole webUserRole, WebUserRole updatedRole)
            {
                webUserRole.IsApproved = updatedRole.IsApproved;
                webUserRole.Status = updatedRole.IsApproved && updatedRole.Status;
                webUserRole.LastModifiedDate = now;

                foreach (var n in webUserRole.Notification)
                {
                    n.Status = false;
                    n.LastModifiedDate = now;
                }
            }
        }

        private bool CheckAgencyIsCisac(string agencyId)
        {
            return !string.IsNullOrWhiteSpace(agencyId) && agencyId == CisacRole;
        }

        private async Task<bool> CisacHasOtherManageRole(WebUser user)
        {
            var cisacUsers = await webUserRepository.FindManyAsyncOptimizedByPath(x => x.AgencyId == user.AgencyId && x.Email != user.Email && x.Status,
                    $"{nameof(WebUserRole)}");

            return cisacUsers.Any(x => x.WebUserRole.Any(y => y.RoleId == (int)PortalRoleType.ManageRoles && y.Status));
        }
    }
}
