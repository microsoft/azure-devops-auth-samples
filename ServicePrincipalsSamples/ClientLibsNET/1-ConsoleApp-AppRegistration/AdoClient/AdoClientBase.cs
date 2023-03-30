using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;

namespace ServicePrincipalsSamples.AdoClient
{
    /// <summary>
    /// Azure DevOps REST client using client libs
    /// </summary>
    /// <typeparam name="T">Azure DevOps HTTP Client</typeparam>
    public abstract class AdoClientBase<T> where T : VssHttpClientBase
    {
        protected readonly AdoConnection adoConnection;
        private T client;

        protected AdoClientBase(AdoConnection adoConnection)
        {
            this.adoConnection = adoConnection;
        }

        public Identity GetAuthorizedIdentity()
        {
            return adoConnection.VssConnection.AuthorizedIdentity;
        }

        protected T GetClient()
        {
            client ??= adoConnection.VssConnection.GetClient<T>();
            return client;
        }
    }
}
