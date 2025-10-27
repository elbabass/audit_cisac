using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IInstrumentationRepository : IRepository<Instrumentation> { }

    internal class InstrumentationRepository : BaseRepository<Instrumentation>, IInstrumentationRepository
    {
        public InstrumentationRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}