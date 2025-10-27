using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IAgreementRepository : IRepository<Agreement> { }

    internal class AgreementRepository : BaseRepository<Agreement>, IAgreementRepository
    {
        public AgreementRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}