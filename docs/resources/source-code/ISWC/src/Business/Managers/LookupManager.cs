using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.Matching;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface ILookupManager
    {
        Task<Instrumentation> GetInstrumentationByCodeAsync(string instrumentationCode);
        Task<IEnumerable<InterestedPartyModel>> GetIps(InterestedPartySearchModel interestedPartySearchModel);
        Task<string> GetPublisherSubmitterCode(long publisherNameNumber);
        Task<string> GetPublisherNumberType(string publisherSubmitterCode);
        Task<string> GetAgencyDisallowDisambiguationOverwrite(string agencyCode);
        Task<IEnumerable<LookupData>> GetLookups();
    }

    public class LookupManager : ILookupManager
    {
        private readonly IInstrumentationRepository instrumentationRepository;
        private readonly IMapper mapper;
        private readonly IMatchingService matchingService;
        private readonly ILookupRepository lookupRepository;
        private readonly ICacheClient cacheClient;
        private readonly IPublisherCodeRespository publisherCodeRespository;
        private readonly INumberTypeRepository numberTypeRepository;
        private readonly IAgencyRepository agencyRepository;

        public LookupManager(
            IInstrumentationRepository instrumentationRepository,
            IMapper mapper,
            IMatchingService matchingService,
            ILookupRepository lookupRepository,
            ICacheClient cacheClient,
            IPublisherCodeRespository publisherCodeRespository,
            INumberTypeRepository numberTypeRepository,
            IAgencyRepository agencyRepository)
        {
            this.instrumentationRepository = instrumentationRepository;
            this.mapper = mapper;
            this.matchingService = matchingService;
            this.lookupRepository = lookupRepository;
            this.cacheClient = cacheClient;
            this.publisherCodeRespository = publisherCodeRespository;
            this.numberTypeRepository = numberTypeRepository;
            this.agencyRepository = agencyRepository;
        }

        public async Task<Instrumentation> GetInstrumentationByCodeAsync(string instrumentationCode)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<Instrumentation>(await instrumentationRepository.FindAsync(i => i.Code == instrumentationCode));
            }, keys: instrumentationCode);
        }

        public async Task<IEnumerable<InterestedPartyModel>> GetIps(InterestedPartySearchModel interestedPartySearchModel)
        {
            return await matchingService.MatchContributorsAsync(interestedPartySearchModel);
        }

        public async Task<string> GetPublisherSubmitterCode(long publisherNameNumber)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<string>((await publisherCodeRespository.FindAsync(p => p.IpnameNumber == publisherNameNumber))?.Code);
            }, keys: publisherNameNumber.ToString());
        }

        public async Task<string> GetPublisherNumberType(string publisherSubmitterCode)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<string>((await numberTypeRepository.FindAsync(n => n.Code == publisherSubmitterCode))?.Code);
            }, keys: publisherSubmitterCode);
        }

        public async Task<string> GetAgencyDisallowDisambiguationOverwrite(string agencyCode)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<string>((await agencyRepository.FindAsync(n => n.AgencyId == agencyCode))?.DisallowDisambiguateOverwrite);
            }, keys: agencyCode);
        }

        public async Task<IEnumerable<LookupData>> GetLookups()
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return await lookupRepository.GetLookups();
            });
        }
    }
}
