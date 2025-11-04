using SpanishPoint.Azure.Iswc.Framework.Mail.Models;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Mail
{
    public interface IMailClient
    {
        Task SendMail(MailMessageModel mailMessage);
    }
}
