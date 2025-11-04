using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IAgencyRepository : IRepository<Agency> { }

    internal class AgencyRepository : BaseRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}