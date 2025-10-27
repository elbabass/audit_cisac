using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Data.DataModels;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IAdditionalIdentifierRepository : IRepository<AdditionalIdentifier> { }

    internal class AdditionalIdentifierRepository : BaseRepository<AdditionalIdentifier>, IAdditionalIdentifierRepository
    {
        public AdditionalIdentifierRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}