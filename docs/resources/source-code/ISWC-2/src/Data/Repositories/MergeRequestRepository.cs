using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IMergeRequestRepository : IRepository<MergeRequest> { }
    internal class MergeRequestRepository : BaseRepository<MergeRequest>, IMergeRequestRepository
    {
        public MergeRequestRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}
