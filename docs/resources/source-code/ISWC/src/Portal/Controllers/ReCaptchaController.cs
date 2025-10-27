using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Client;
using SpanishPoint.Azure.Iswc.Portal.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Portal.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReCaptchaController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly IRestApiIdentityServerClient identityServerClient;

        public ReCaptchaController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IRestApiIdentityServerClient identityServerClient)
        {
            httpClient = httpClientFactory.CreateClient("GoogleReCaptcha");
            this.configuration = configuration;
            this.identityServerClient = identityServerClient;
        }

        [HttpPost]
        public async Task<IActionResult> ValidateReCaptchaResponse(string responseToken)
        {
            var parameters = new Dictionary<string, string> { { "secret", configuration["Recaptcha-SiteKey"] }, { "response", responseToken } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await (await httpClient.PostAsync("", encodedContent)).EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            response = response.AddPropertyToJson("token_id", await identityServerClient.RequestClientCredentialsTokenAsync(
                new UserDetailsModel
                {
                    AgentID = "1000"
                }.AsDictionary()));

            return Json(response);
        }
    }
}
