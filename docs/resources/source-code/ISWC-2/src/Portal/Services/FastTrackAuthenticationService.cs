using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Portal.Controllers;
using SpanishPoint.Azure.Iswc.Portal.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpanishPoint.Azure.Iswc.Portal.Services
{
    public interface IFastTrackAuthenticationService
    {
        Task<UserDetailsModel> AuthenticateUser(UserAutheticationModel model);
        Task<string> LoginUser(UserAutheticationModel model);
    }

    internal class FastTrackAuthenticationService : IFastTrackAuthenticationService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<FastTrackAuthenticationService> logger;

        public FastTrackAuthenticationService(IHttpClientFactory httpClientFactory, ILogger<FastTrackAuthenticationService> logger)
        {
            httpClient = httpClientFactory.CreateClient("FastTrackAuthenticationService");
            this.logger = logger;
        }

        public async Task<UserDetailsModel> AuthenticateUser(UserAutheticationModel model)
        {
            var xmlBody = $@"
                <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:sec='http://security.fasttrackdcn.net/'>
                    <soapenv:Header>
                        <clientIdentifier>
                            <location><name>058</name></location>
                            <name>Gddn</name>
                            <version>4.0.0</version>
                        </clientIdentifier>
                    </soapenv:Header>
                    <soapenv:Body>
                        <sec:authenticateUserWithPassport>
                            <userMail>{ model.UserId }</userMail>
                            <userPassport>{ model.Passport }</userPassport>
                            <trackId>FTGDDN</trackId>
                        </sec:authenticateUserWithPassport>
                    </soapenv:Body>
                </soapenv:Envelope>";

            var response = await httpClient.PostAsync("", new StringContent(xmlBody, Encoding.UTF8, "application/xml"));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger.LogError(await response.Content.ReadAsStringAsync());
                return null;
            }

            var xml = XDocument.Parse(await response.Content.ReadAsStringAsync());

            return new UserDetailsModel
            {
                AgentID = xml.Descendants("societyCode").FirstOrDefault().Value,
                Email = xml.Descendants("mail").FirstOrDefault().Value
            };
        }

        public async Task<string> LoginUser(UserAutheticationModel model)
        {
            var xmlBody = $@"
                <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:sec='http://security.fasttrackdcn.net/'>
                  <soapenv:Header>
                     <clientIdentifier>
                        <location><name>058</name></location>
                        <name>Gddn</name>
                        <version>5.2.0</version>
                     </clientIdentifier>
                  </soapenv:Header>
                  <soapenv:Body>
                     <sec:authenticateUser>
                        <userMail>{model.UserId}</userMail>
                        <userPassword>{model.Password}</userPassword>
                        <trackId>FTGDDN</trackId>
                     </sec:authenticateUser>
                  </soapenv:Body>
                </soapenv:Envelope>";

            var response = (await httpClient.PostAsync("", new StringContent(xmlBody, Encoding.UTF8, "application/xml"))).EnsureSuccessStatusCode();

            var xml = await response.Content.ReadAsStringAsync();
            return XDocument.Parse(xml).Descendants("passport").FirstOrDefault().Value;
        }
    }
}
