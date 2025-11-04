using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface ISubmissionSourceRepository : IRepository<SubmissionSource> { }

    internal class SubmissionSourceRepository : BaseRepository<SubmissionSource>, ISubmissionSourceRepository
    {
        public SubmissionSourceRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}