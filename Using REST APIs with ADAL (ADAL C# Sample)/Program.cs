using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace SimpleAdalConsoleApp
{
    class Program
    { 
        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798"; //Static value to target VSTS. Do not change
        internal const string clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1"; //VS ClientId. Please use this instead of your app's clientId
        internal const string VSTSAccountName = "myaccount"; //change to domain of your VSTS account (e.g. "myaccount" when account name is myaccount.visualstudio.com) 

        static void Main(string[] args)
        {
            int iteration = 0;
            
            AuthenticationContext ctx = GetAuthenticationContext(null);
            AuthenticationResult result = null;
            try
            {
                //PromptBehavior.RefreshSession will enforce an authn prompt every time. NOTE: Auto will take your windows login state if possible
                result = ctx.AcquireTokenAsync(VSTSResourceId, clientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Auto)).Result;
                Console.WriteLine("Token expires on: " + result.ExpiresOn);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

            do
            {
                Console.WriteLine("({1}) Iteration {0}", iteration++, DateTime.UtcNow);

                var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                try
                {
                    ListProjects(VSTSAccountName, bearerAuthHeader);
                    Thread.Sleep(300000); //Sleep for 5 minutes
                }
                catch (UnauthorizedAccessException uae)
                {
                    //prompts user with a login prompt, so they can login with a different user when receiving a 401 for the last authenticated user
                    result = ctx.AcquireTokenAsync(VSTSResourceId, clientId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Always)).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
                }
            }
            while (true);
        }

        private static AuthenticationContext GetAuthenticationContext(string tenant)
        {
            AuthenticationContext ctx = null;
            if (tenant != null)
                ctx = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            else
            {
                ctx = new AuthenticationContext("https://login.windows.net/common");
                if (ctx.TokenCache.Count > 0)
                {
                    string homeTenant = ctx.TokenCache.ReadItems().First().TenantId;
                    ctx = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
                }
            }

            return ctx;
        }

        private static void ListProjects(string vstsAccountName, AuthenticationHeaderValue authHeader)
        {
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(String.Format("https://{0}.visualstudio.com", vstsAccountName));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\tSuccesful REST call");
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                }
            }
        }
    }
}
