using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DeviceProfileSample
{
    public class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string vstsCollectionUrl = "http://myaccount.visualstudio.com"; //change to the URL of your VSTS account
        //==========================================================================

        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798"; //Static value to target VSTS. Do not change
        internal const string clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1"; //VS ClientId. Please use this instead of your app's clientId

        public static void Main(string[] args)
        {
            AuthenticationContext ctx = GetAuthenticationContext(null);
            AuthenticationResult result = null;
            try
            {
                DeviceCodeResult codeResult = ctx.AcquireDeviceCodeAsync(VSTSResourceId, clientId).Result;
                result = ctx.AcquireTokenByDeviceCodeAsync(codeResult).Result;
                Console.WriteLine("Token expires on: " + result.ExpiresOn);

                var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                ListProjects(VSTSAccountName, bearerAuthHeader);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong.");
                Console.WriteLine("Message: " + exc.Message + "\n");
            }
            return result;
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
                client.BaseAddress = new Uri(vstsCollectionUrl);
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
