using System;

namespace SpanishPoint.Azure.Iswc.Portal.Configuration.Options
{
    public class ClientAppOptions
    {
        public Uri LoginRedirectUri { get; set; }
        public string RecaptchaPublicKey { get; set; }
        public string ApplicationInsightsKey { get; set; }
        public Uri IswcApiManagementUri { get; set; }
    }
}
