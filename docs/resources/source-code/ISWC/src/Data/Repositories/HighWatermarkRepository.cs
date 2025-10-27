using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IHighWatermarkRepository : IRepository<HighWatermark> { }

    internal class HighWatermarkRepository : BaseRepository<HighWatermark>, IHighWatermarkRepository
    {
        public HighWatermarkRepository() { }
        public HighWatermarkRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}
