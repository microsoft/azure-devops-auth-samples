using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ManagedClientConsoleAppSample
{
    //After running the console will close so please add a breakpoint or sleep to see output.
    class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string vstsCollectionUrl = "https://guidedultrasonics.visualstudio.com"; //change to the URL of your VSTS account; NOTE: This must use HTTPS
        internal const string clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";          //change to your app registration's Application ID, unless you are an MSA backed account
        internal const string replyUri = "urn:ietf:wg:oauth:2.0:oob";                     //change to your app registration's reply URI, unless you are an MSA backed account
        //==========================================================================

        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798"; //Constant value to target VSTS. Do not change  

        public static void Main(string[] args)
        {
            AuthenticationContext ctx = GetAuthenticationContext(null);
            AuthenticationResult result = null;

#if NET452
            IPlatformParameters p = new PlatformParameters(PromptBehavior.Always);
#else
            IPlatformParameters p = new PlatformParameters();
#endif
            try
            {

                //PromptBehavior.RefreshSession will enforce an authn prompt every time. NOTE: Auto will take your windows login state if possible
                result = ctx.AcquireTokenAsync(VSTSResourceId, clientId, new Uri(replyUri), p).Result;
                Console.WriteLine("Token expires on: " + result.ExpiresOn);

                var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                ListProjects(bearerAuthHeader);
            }
            catch (UnauthorizedAccessException)
            {
                // If the token has expired, prompt the user with a login prompt
                result = ctx.AcquireTokenAsync(VSTSResourceId, clientId, new Uri(replyUri), p).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", ex.GetType(), ex.Message);
            }
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

        private static void ListProjects(AuthenticationHeaderValue authHeader)
        {
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "ManagedClientConsoleAppSample");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\tSuccesful REST call");
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
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
