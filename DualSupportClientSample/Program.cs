using System;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;

namespace DualSupportClientSample
{
    //After running the console will close so please add a breakpoint or sleep to see output.
    class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string azDevOrganizationUrl = "https://myaccount.visualstudio.com"; //change to the URL of your VSTS or TFS account; VSTS: https://*.visualstudio.com TFS: https://*:8080/tfs/defaultcollection" 
        //==========================================================================

        public static void Main(string[] args)
        {
            try
            {
                //Based on collection URL will either start an interactive login session or use local Windows credential authentication
                VssConnection connection = new VssConnection(new Uri(azDevOrganizationUrl), new VssClientCredentials());

                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
                IEnumerable<TeamProjectReference> projects = projectClient.GetProjects().Result;
                foreach (TeamProjectReference p in projects)
                {
                    Console.WriteLine(p.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
        }
    }
}
