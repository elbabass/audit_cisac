using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface ILinkedToManager
    {
        Task<bool> MergeExists(string preferredIswc, WorkNumber workNumber);
    }

    public class LinkedToManager : ILinkedToManager
    {
        private readonly IIswcLinkedToRepository iswcLinkedTo;

        public LinkedToManager(IIswcLinkedToRepository iswcLinkedTo)
        {
            this.iswcLinkedTo = iswcLinkedTo;
        }

        public async Task<bool> MergeExists(string preferredIswc, WorkNumber workNumber)
        {
            return await iswcLinkedTo.ExistsAsync(x => x.LinkedToIswc == preferredIswc && x.Status &&
                x.Iswc.WorkInfo.Any(w => w.AgencyWorkCode == workNumber.Number && w.AgencyId == workNumber.Type));
        }

    }
}
