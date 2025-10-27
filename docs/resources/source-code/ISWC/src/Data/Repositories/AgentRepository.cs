using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IAgentRepository : IRepository<AgentVersion> { }

    internal class AgentRepository : BaseRepository<AgentVersion>, IAgentRepository
    {
        public AgentRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
    }
}
