using Microsoft.VisualStudio.Services.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ServicePrincipalsSamples.Aad
{
    /// <summary>
    /// Adds an Azure AD access token for Azure DevOps as authentication mechanism for every request
    /// </summary>
    public class AadAccessTokenHandler : DelegatingHandler
    {
        private readonly AadClient _aadClient;

        public AadAccessTokenHandler(AadClient aadClient)
        {
            _aadClient = aadClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await _aadClient.GetAadAccessToken(VssAadSettings.DefaultScopes);
            request.Headers.Authorization = new AuthenticationHeaderValue(result.TokenType, result.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
