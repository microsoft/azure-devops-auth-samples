using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using ServicePrincipalsSamples.Aad;
using ServicePrincipalsSamples.Settings;
using System;
using System.Net.Http;

namespace ServicePrincipalsSamples.AdoClient
{
    /// <summary>
    /// Wraps the Azure DevOps connection with the supported authentication mechanisms on this application
    /// </summary>
    public class AdoConnection : IDisposable
    {
        /// <summary>
        /// IMPORTANT: The VssConnection instance should be a singleton in the application.
        /// </summary>
        public VssConnection VssConnection { get; private set; }

        private bool disposedValue;

        public AdoConnection(AppConfiguration config, AadClient aadClient)
        {
            VssConnection = CreateVssConnection(config, aadClient);
        }

        private static VssConnection CreateVssConnection(AppConfiguration config, AadClient aadClient)
        {
            VssCredentials credentials;

            if (config.Ado.AdoAuthenticationMode == AdoAuthenticationMode.AdoPat)
            {
                credentials = CreateVssConnectionWithPAT(config);
            }
            else if (config.Ado.AdoAuthenticationMode == AdoAuthenticationMode.AadServicePrincipal)
            {
                credentials = CreateVssConnectionWithAadAccessToken(aadClient);
            } else
            {
                throw new InvalidOperationException($"Unsupported authentication mode: {config.Ado.AdoAuthenticationMode}");
            }

            var settings = VssClientHttpRequestSettings.Default.Clone();
            // Custom UserAgent with format: "<client lib user agent> <AppUserAgent>")
            // E.g.: "VSServices/16.170.30907.1 (NetStandard; Microsoft Windows 10.0.22621) Identity.ServicePrincipalsSamples/1.0 (1-ConsoleApp-AppRegistration)"
            settings.UserAgent = AppConfiguration.AppUserAgent;

            var innerHandlers = new VssHttpMessageHandler(credentials, settings);

            var delegatingHandlers = new DelegatingHandler[] { new AdoRequestHandler() };

            return new VssConnection(config.Ado.OrganizationUrl, innerHandlers, delegatingHandlers);
        }

        /// <summary>
        /// Creates credentials with an Azure AD Service Prinicpal acces token as authentication mechanism. 
        /// The token regeneration once it is expired is handled by the AadClient.
        /// </summary>
        /// <param name="aadClient">Azure AD client</param>
        /// <returns></returns>
        private static VssCredentials CreateVssConnectionWithAadAccessToken(AadClient aadClient)
        {
            var vssAadToken = new VssAadToken((scopes) => aadClient.GetAadAccessToken(scopes).SyncResultConfigured());
            return new VssAadCredential(vssAadToken);
        }

        /// <summary>
        /// Creates credentials with an Azure DevOps PAT as authentication mechanism
        /// </summary>
        /// <param name="config">app configuration</param>
        /// <returns></returns>
        private static VssCredentials CreateVssConnectionWithPAT(AppConfiguration config)
        {
            return new VssBasicCredential(string.Empty, config.Ado.Pat);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    VssConnection.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}
