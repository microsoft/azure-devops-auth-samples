using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePrincipalsSamples.AdoClient
{
    public class GraphClient : AdoClientBase<GraphHttpClient>
    {
        public GraphClient(AdoConnection adoConnection) : base(adoConnection) { }

        public async Task<GraphUser> AddAadUserToGroup(string adoGroupSD, string userObjectId)
        {
            var groupDescriptors = new[] { SubjectDescriptor.FromString(adoGroupSD) };

            var userContext = new GraphUserOriginIdCreationContext()
            {
                OriginId = userObjectId,
            };

            return await GetClient().CreateUserAsync(userContext, groupDescriptors);
        }

        /// <summary>
        /// Returns Azure AD users in the organization
        /// </summary>
        /// <param name="pages">Number of pages to return (0 means all pages)</param>
        /// <returns>Azure AD users in the organization</returns>
        public async Task<IEnumerable<GraphUser>> ListAadUsers(int pages = 0)
        {
            int pageCounter = 0;
            string continuationToken = null;
            IList<GraphUser> users = new List<GraphUser>();

            do
            {
                var page = await GetClient().ListUsersAsync(subjectTypes: new[] { "aad" }, continuationToken);
                users.AddRange(page.GraphUsers);
                continuationToken = page.ContinuationToken?.First();
                pageCounter++;
            } while ((pages == 0 || pageCounter < pages) && continuationToken != null);

            return users;
        }
    }
}
