using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
	public interface IPublisherCodeRespository: IRepository<PublisherSubmitterCode> { }
	internal class PublisherCodeRepository: BaseRepository<PublisherSubmitterCode>, IPublisherCodeRespository
	{
		public PublisherCodeRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
	}
}
