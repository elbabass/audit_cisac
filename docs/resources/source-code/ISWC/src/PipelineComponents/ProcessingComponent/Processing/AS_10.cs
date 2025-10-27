using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public interface IAS_10
    {
        Task<Submission> RecaculateAuthoritativeFlag(Submission submission);
    }
    public class AS_10 : IAS_10
    {
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IRulesManager rulesManager;

        public AS_10(IInterestedPartyManager interestedPartyManager, IRulesManager rulesManager)
        {
            this.interestedPartyManager = interestedPartyManager;
            this.rulesManager = rulesManager;
        }

        public async Task<Submission> RecaculateAuthoritativeFlag(Submission submission)
        {
            var eligibleAgencies = ValidationRuleConverter.GetValues_IncludeAgenciesInEligibilityCheck(
               await rulesManager.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck"), submission.Model.Agency);

            foreach (var ip in submission.Model.InterestedParties.Where(c => c.CisacType == CisacInterestedPartyType.C ||
                c.CisacType == CisacInterestedPartyType.MA || c.CisacType == CisacInterestedPartyType.TA))
            {
                if (submission.Model.Agency != null && await interestedPartyManager.IsAuthoritative(ip, eligibleAgencies))
                    ip.IsAuthoritative = true;

                else
                    ip.IsAuthoritative = false;
            }

            return submission;
        }
    }
}
