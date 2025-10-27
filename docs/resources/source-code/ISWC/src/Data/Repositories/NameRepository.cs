using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface INameRepository : IRepository<Name> { }

    internal class NameRepository : BaseRepository<Name>, INameRepository
    {
        public NameRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}