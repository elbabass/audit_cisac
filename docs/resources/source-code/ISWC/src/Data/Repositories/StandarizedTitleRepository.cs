using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IStandardizedTitleRepository : IRepository<StandardizedTitle> { }

    internal class StandardizedTitleRepository : BaseRepository<StandardizedTitle>, IStandardizedTitleRepository
    {
        public StandardizedTitleRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}