using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using ServicePrincipalsSamples.Settings;
using System;
using System.Threading.Tasks;

namespace ServicePrincipalsSamples.Aad
{
    /// <summary>
    /// Azure AD client for a App Registration Service Principal
    /// </summary>
    public class AadClient
    {
        private const string ClientSecretPlaceholderValue = "[Enter here a client secret for your application]";

        private IConfidentialClientApplication app;

        public AadClient(AppConfiguration config)
        {
            Initialize(config);
        }

        private void Initialize(AppConfiguration config)
        {
            if (IsAppUsingClientSecret(config))
            {
                app = ConfidentialClientApplicationBuilder.Create(config.Aad.ClientId)
                    .WithClientSecret(config.Aad.ClientSecret)
                    .WithAuthority(config.Aad.Authority)
                    .Build();
            }
            else
            {
                ICertificateLoader certificateLoader = new DefaultCertificateLoader();
                certificateLoader.LoadIfNeeded(config.Aad.Certificate);

                app = ConfidentialClientApplicationBuilder.Create(config.Aad.ClientId)
                    .WithCertificate(config.Aad.Certificate.Certificate)
                    .WithAuthority(config.Aad.Authority)
                    .Build();
            }

            app.AddInMemoryTokenCache();
        }

        private static bool IsAppUsingClientSecret(AppConfiguration config)
        {
            if (!string.IsNullOrWhiteSpace(config.Aad.ClientSecret) && config.Aad.ClientSecret != ClientSecretPlaceholderValue)
            {
                return true;
            }
            else if (config.Aad.Certificate != null)
            {
                return false;
            }
            else
            {
                throw new ArgumentException("You must choose between using secret or certificate. Please update appsettings.json file.");
            }
        }

        /// <summary>
        /// Returns an Azure AD access token (client credentials). It uses an in-memory cache and it also regenerates the access token if it is expired. 
        /// </summary>
        /// <returns>Valid Azure AD access token</returns>
        public async Task<AuthenticationResult> GetAadAccessToken(string[] scopes)
        {
            // Client credentials flow uses the cache by default
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Token acquired for the Service Principal (source: '{result.AuthenticationResultMetadata.TokenSource}')\n");
            Console.ResetColor();

            return result;
        }
    }
}