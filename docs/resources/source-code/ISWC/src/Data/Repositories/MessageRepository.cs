using Microsoft.AspNetCore.Http;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
	public interface IMessageRepository : IRepository<Message> { }
	internal class MessageRepository: BaseRepository<Message>, IMessageRepository
	{
		public MessageRepository(CsiContext context, IHttpContextAccessor httpContext) : base(context, httpContext) { }
	}
}
