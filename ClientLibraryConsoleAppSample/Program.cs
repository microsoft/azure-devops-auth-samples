using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ClientLibraryConsoleAppSample
{
    class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string vstsCollectionUrl = "http://myaccount.visualstudio.com"; //change to the URL of your VSTS account
        // internal const string vstsCollectioUrl = "http://myserver:8080/tfs/DefaultCollection" alternate URL for a TFS collection
        //==========================================================================

        //Console application to execute a user defined work item query
        static void Main(string[] args)
        {
            //Prompt user for credential for collection specified above
            VssCredentials cred = new VssClientCredentials(false);
            cred.PromptType = CredentialPromptType.PromptIfNeeded;
            VssConnection connection = new VssConnection(new Uri(vstsCollectionUrl), cred);

            //create http client and query for resutls
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            Wiql query = new Wiql() { Query = "Select [State], [Title] from WorkItems where [Work Item Type] = 'Bug' And [Tags] Contains 'findMe'" };
            WorkItemQueryResult queryResults = witClient.QueryByWiqlAsync(query).Result;

            //Display reults in console
            if (queryResults == null || queryResults.WorkItems.Count() == 0)
            {
                Console.WriteLine("Query did not find any results");
            }
            else
            {
                foreach (var item in queryResults.WorkItems)
                {
                    Console.WriteLine(item.Id);
                }
            }
        }
    }
}
