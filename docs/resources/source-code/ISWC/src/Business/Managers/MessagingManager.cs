using Microsoft.Extensions.Localization;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Resources;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IMessagingManager
    {
        Task<Rejection> GetRejectionMessage(ErrorCode errorCode);
    }

    public class MessagingManager : IMessagingManager
    {
        private readonly IStringLocalizer localizer;

        public MessagingManager(IStringLocalizer<ApiErrorCodes> localizer)
        {
            this.localizer = localizer;
        }

        public Task<Rejection> GetRejectionMessage(ErrorCode errorCode)
        {
            return Task.FromResult(new Rejection(errorCode, GetErrorMsgByCode(errorCode)));

            string GetErrorMsgByCode(ErrorCode code)
            {
                return localizer[code.ToString().Replace("_", "")].Value;
            }
        }
    }
}
