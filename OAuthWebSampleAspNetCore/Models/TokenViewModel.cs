using System;

namespace OAuthWebSampleAspNetCore.Models
{
    public class TokenViewModel
    {
        public Token Token { get; set; }

        public string Error { get; set; }

        public OAuthSettings OAuthSettings { get; set; }        
    }
}