using System;

namespace OAuthWebSampleAspNetCore.Models
{
    public class OAuthSettings
    {
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public string ProfileUrl { get; set; }
        public ClientAppSettings ClientApp { get; set; }
    }

    public class ClientAppSettings
    {
        public Guid Id { get; set; }
        public string Scope { get; set; }
        public string CallbackUrl { get; set; }        
    }
}
