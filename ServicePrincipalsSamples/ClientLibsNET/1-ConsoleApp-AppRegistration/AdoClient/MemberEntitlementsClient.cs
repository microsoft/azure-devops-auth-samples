using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.MemberEntitlementManagement.WebApi;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Common;

namespace ServicePrincipalsSamples.AdoClient
{
    public class MemberEntitlementsClient : AdoClientBase<MemberEntitlementManagementHttpClient>
    {
        public MemberEntitlementsClient(AdoConnection adoConnection) : base(adoConnection) { }

        /// <summary>
        /// Returns users in the organization
        /// </summary>
        /// <param name="pages">Number of pages to return (0 means all pages)</param>
        /// <returns>users in the organization</returns>
        public async Task<IList<UserEntitlement>> SearchUserEntitlements(int pages = 0)
        {
            int pageCounter = 0;
            string continuationToken = null;
            IList<UserEntitlement> users = new List<UserEntitlement>();

            do
            {
                var page = await GetClient().SearchUserEntitlementsAsync(continuationToken, orderBy: "name");
                users.AddRange(page.Members);
                continuationToken = page.ContinuationToken;
                pageCounter++;
            } while ((pages == 0 || pageCounter < pages) && continuationToken != null);

            return users;
        }

        public async Task<ServicePrincipalEntitlement> GetServicePrincipalEntitlement(Guid servicePrinicipalId)
        {
            return await GetClient().GetServicePrincipalEntitlementAsync(servicePrinicipalId);
        }

        public async Task<ServicePrincipalEntitlement> GetServicePrincipalEntitlementMe()
        {
            var servicePrincipalId = adoConnection.VssConnection.AuthorizedIdentity.Id;
            return await GetServicePrincipalEntitlement(servicePrincipalId);
        }
    }
}
