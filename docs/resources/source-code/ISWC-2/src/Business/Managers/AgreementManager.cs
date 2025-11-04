using AutoMapper;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IAgreementManager
    {
        Task<IEnumerable<Agreement>> FindManyAsync(IEnumerable<string> ipBaseNumbers);
    }

    public class AgreementManager : IAgreementManager
    {
        private readonly IAgreementRepository agreementRepository;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;

        public AgreementManager(IAgreementRepository agreementRepository, IMapper mapper, ICacheClient cacheClient)
        {
            this.agreementRepository = agreementRepository;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
        }

        public async Task<IEnumerable<Agreement>> FindManyAsync(IEnumerable<string> ipBaseNumbers)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<IEnumerable<Agreement>>(await agreementRepository.FindManyAsyncOptimized(
                    a => ipBaseNumbers.Contains(a.IpbaseNumber),
                    i => i.Agency));
            }, keys: ipBaseNumbers.ToArray());
        }
    }
}
