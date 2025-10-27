using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using SpanishPoint.Azure.Iswc.Framework.Mail.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Auth;
using System.Diagnostics.CodeAnalysis;

namespace SpanishPoint.Azure.Iswc.Framework.Mail.Graph
{
	[ExcludeFromCodeCoverage]
	public class GraphMailClient : IMailClient
	{
		private readonly GraphServiceClient graphClient;

		public GraphMailClient(IOptions<GraphMailClientOptions> options)
		{
			var confidentialClientApplication = ConfidentialClientApplicationBuilder
				.Create(options.Value.ClientID)
				.WithTenantId(options.Value.Tenant)
				.WithClientSecret(options.Value.Secret)
				.Build();

			var authProvider = new ClientCredentialProvider(confidentialClientApplication);
			graphClient = new GraphServiceClient(authProvider);
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
			
			await graphClient
			.Users[sender]
			.SendMail(new Message
			{
				Subject = subject,
				Body = new ItemBody
				{
					ContentType = BodyType.Text,
					Content = content
				},
				ToRecipients = new List<Recipient>()
				{
						new Recipient
						{
							EmailAddress=new EmailAddress
							{
								Address = mailMessage.Recipient
							}
						}
				}
			})
			.Request()
			.PostAsync();
		}
	}
}
