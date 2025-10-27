using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using System;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IWebUserRepository : IRepository<WebUser>
    {
    }

    internal class WebUserRepository : BaseRepository<WebUser>, IWebUserRepository
    {
        public WebUserRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
       
    }
}
