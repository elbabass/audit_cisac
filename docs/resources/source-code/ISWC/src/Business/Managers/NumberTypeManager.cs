using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface INumberTypeManager
    {
        Task<bool> Exists(string submitterCode);
        Task<NumberType> FindAsync(string submitterCode);
    }


    public class NumberTypeManager : INumberTypeManager
    {
        private readonly INumberTypeRepository numberTypeRepository;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;

        public NumberTypeManager(INumberTypeRepository numberTypeRepository, IMapper mapper, ICacheClient cacheClient)
        {
            this.numberTypeRepository = numberTypeRepository;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
        }

        public async Task<bool> Exists(string submitterCode)
        {
            return await numberTypeRepository.ExistsAsync(x => x.Code == submitterCode);
        }

        public async Task<NumberType> FindAsync(string submitterCode)
        {
            return await numberTypeRepository.FindAsync(x => x.Code == submitterCode);
        }
    }
}
