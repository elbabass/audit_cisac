using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Portal;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
	public interface IPortalMessageManager
	{
		Task<IEnumerable<Message>> GetPortalMessages(string culture, string env);
	}

	class PortalMessageManager: IPortalMessageManager
	{
		private readonly IMessageRepository messageRepository;
		private readonly IMapper mapper;
		public PortalMessageManager(IMessageRepository messageRepository, IMapper mapper)
		{
			this.messageRepository = messageRepository;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<Message>> GetPortalMessages(string env, string culture)
		{
			var messages = await messageRepository.FindManyAsync(x => x.LanguageType.LanguageCode == culture && x.Portal == env && x.Status == true);
			return mapper.Map<IEnumerable<Message>>(messages);
		}
	}
}
