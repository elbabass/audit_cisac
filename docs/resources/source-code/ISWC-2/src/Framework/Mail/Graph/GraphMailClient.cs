using Microsoft.Extensions.Options;
using Microsoft.Graph;
using SpanishPoint.Azure.Iswc.Framework.Mail.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using Azure.Identity;

namespace SpanishPoint.Azure.Iswc.Framework.Mail.Graph
{
    [ExcludeFromCodeCoverage]
    public class GraphMailClient : IMailClient
    {
        private readonly GraphServiceClient graphClient;

        public GraphMailClient(IOptions<GraphMailClientOptions> options)
        {
            var clientSecretCredential = new ClientSecretCredential(
                options.Value.Tenant,
                options.Value.ClientID,
                options.Value.Secret
            );

            graphClient = new GraphServiceClient(clientSecretCredential);
        }

        public async Task SendMail(MailMessageModel mailMessage)
        {
            string sender = "iswc.reporting@cisac.org";
            string subject = "Report ready for download.";
            string content = $"Your {mailMessage.ReportType} report is ready for download from FTP.";

            if (mailMessage.AgentRunInformation != null)
            {
                sender = "iswc.agent@cisac.org";
                subject = "Agent Run Alert";
                content = $"Agent run with ID '{mailMessage.AgentRunInformation.RunId}' for agency '{mailMessage.AgentRunInformation.AgencyCode}' did not finish within the acceptable timeframe.";
            }

            var body = new SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content = content
                    },
                    ToRecipients = new List<Recipient>
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = mailMessage.Recipient
                            }
                        }
                    }
                }
            };

            await graphClient
            .Users[sender]
            .SendMail
            .PostAsync(body);
        }
    }
}