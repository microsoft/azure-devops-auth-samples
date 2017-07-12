using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace ClientLibraryConsoleAppSample
{
    class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string vstsCollectionUrl = "https://myaccount.visualstudio.com"; //change to the URL of your VSTS account; NOTE: This must use HTTPS
        // internal const string vstsCollectioUrl = "http://myserver:8080/tfs/DefaultCollection" alternate URL for a TFS collection
        //==========================================================================

        //Console application to execute a user defined work item query
        static void Main(string[] args)
        {
            //Prompt user for credential
            VssConnection connection = new VssConnection(new Uri(vstsCollectionUrl), new VssClientCredentials());

            //create http client and query for resutls
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            Wiql query = new Wiql() { Query = "SELECT [Id], [Title], [State] FROM workitems WHERE [Work Item Type] = 'Bug' AND [Assigned To] = @Me" };
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
