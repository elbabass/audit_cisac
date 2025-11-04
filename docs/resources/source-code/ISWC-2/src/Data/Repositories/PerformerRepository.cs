using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IPerformerRepository : IRepository<Performer> { }

    internal class PerformerRepository : BaseRepository<Performer>, IPerformerRepository
    {
        public PerformerRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}
