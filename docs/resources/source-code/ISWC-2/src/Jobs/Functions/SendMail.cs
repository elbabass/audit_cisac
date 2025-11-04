using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.Mail;
using SpanishPoint.Azure.Iswc.Framework.Mail.Models;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class SendMail
    {
        private readonly IMailClient mailClient;

        public SendMail(IMailClient mailClient)
        {
            this.mailClient = mailClient;
        }

        [Function("SendMail")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            string json = await new StreamReader(req.Body).ReadToEndAsync();

            var model = JsonConvert.DeserializeObject<MailMessageModel>(json);
            await mailClient.SendMail(model);

            var resp = req.CreateResponse(HttpStatusCode.OK);
            return resp;
        }
    }
}
