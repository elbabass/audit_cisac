using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface ISubmissionSourceManager
    {
        Task<SubmissionSourceModel> FindAsync(string agency);
    }

    public class SubmissionSourceManager : ISubmissionSourceManager
    {
        private readonly ISubmissionSourceRepository submissionSourceRepository;
        private readonly IMapper mapper;
        private readonly ICacheClient cacheClient;

        public SubmissionSourceManager(
            ISubmissionSourceRepository submissionSourceRepository,
            IMapper mapper,
            ICacheClient cacheClient)
        {
            this.submissionSourceRepository = submissionSourceRepository;
            this.mapper = mapper;
            this.cacheClient = cacheClient;
        }

        public async Task<SubmissionSourceModel> FindAsync(string agency)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                return mapper.Map<SubmissionSourceModel>(await submissionSourceRepository.FindAsync(x => x.Code == agency));
            }, keys: agency);
        }
    }
}
