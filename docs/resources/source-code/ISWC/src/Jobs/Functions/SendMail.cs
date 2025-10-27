using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.Mail;
using SpanishPoint.Azure.Iswc.Framework.Mail.Models;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class SendMail
    {
        private readonly IMailClient mailClient;

        public SendMail(IMailClient mailClient)
        {
            this.mailClient = mailClient;
        }

        [FunctionName("SendMail")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await mailClient.SendMail(
                JsonConvert.DeserializeObject<MailMessageModel>(
                    await req.ReadAsStringAsync()));
            return new OkObjectResult(null);
        }
    }
}
