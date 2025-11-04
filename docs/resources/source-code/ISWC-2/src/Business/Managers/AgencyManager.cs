using AutoMapper;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IAgencyManager
    {
        Task<string> FindAsync(string agency);
        Task<bool> Exists(string agency);
        Task AddAgency(string agency);
        Task<bool> ChecksumEnabled(string agency);
    }

    public class AgencyManager : IAgencyManager
    {
        private readonly IAgencyRepository agencyRepository;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;

        public AgencyManager(IAgencyRepository agencyRepository, IMapper mapper, ICacheClient cacheClient)
        {
            this.agencyRepository = agencyRepository;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
        }

        public async Task<string> FindAsync(string agency)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<string>((await agencyRepository.FindAsync(a => a.AgencyId == agency))?.AgencyId);
            }, keys: agency);
        }

        public async Task<bool> Exists(string agency)
        {
           return await agencyRepository.ExistsAsync(a => a.AgencyId == agency);          
        }

        public async Task AddAgency(string agency)
        {
            var unitOfWork = agencyRepository.UnitOfWork;
            var now = DateTime.UtcNow;

            var newAgency = new Data.DataModels.Agency
            {
                AgencyId = agency,
                Name = "UNKNOWN",
                LastModifiedDate = now,
                CreatedDate = now,
                Iswcstatus = 1,
                LastModifiedUserId = 5,
                DisallowDisambiguateOverwrite = "REST;FILE;CISNET"
            };

            await agencyRepository.AddAsync(newAgency);
            await unitOfWork.Save();
        }

        public async Task<bool> ChecksumEnabled(string agency)
		{
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<bool>((await agencyRepository.FindAsync(a => a.AgencyId == agency))?.EnableChecksumValidation);
            }, keys: agency);
        }
    }
}
