using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IRecordingRepository : IRepository<Recording> { }
    internal class RecordingRepository : BaseRepository<Recording>, IRecordingRepository
    {
        public RecordingRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}