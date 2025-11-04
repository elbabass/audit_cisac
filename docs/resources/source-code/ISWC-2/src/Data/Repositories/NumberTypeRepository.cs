using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
	public interface INumberTypeRepository : IRepository<NumberType> { }
	internal class NumberTypeRepository : BaseRepository<NumberType>, INumberTypeRepository
	{
		public NumberTypeRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
	}
}
