using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IAdditionalIdentifierManager
    {
        Task<bool> Exists(string identifier);
        Task<AdditionalIdentifier> FindAsync(string identifier);
        Task<IEnumerable<AdditionalIdentifier>> FindManyAsync(IEnumerable<string> identifiers, int numberTypeId);
    }


    public class AdditionalIdentifierManager : IAdditionalIdentifierManager
    {
        private readonly IAdditionalIdentifierRepository additionalIdentifierRepository;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;

        public AdditionalIdentifierManager(IAdditionalIdentifierRepository additionalIdentifierRepository, IMapper mapper, ICacheClient cacheClient)
        {
            this.additionalIdentifierRepository = additionalIdentifierRepository;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
        }

        public async Task<bool> Exists(string identifier)
        {
            return await additionalIdentifierRepository.ExistsAsync(ai => ai.WorkIdentifier == identifier);
        }

        public async Task<AdditionalIdentifier> FindAsync(string identifier)
        {
            return await additionalIdentifierRepository.FindAsync(ai => ai.WorkIdentifier == identifier);
        }

        public async Task<IEnumerable<AdditionalIdentifier>> FindManyAsync(IEnumerable<string> identifiers, int numberTypeId)
        {
            return await additionalIdentifierRepository.FindManyAsync(ai => identifiers.Contains(ai.WorkIdentifier) && ai.NumberTypeId == numberTypeId);
        }
    }
}
