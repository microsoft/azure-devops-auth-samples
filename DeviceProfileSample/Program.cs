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
        public const string resource_vsts = "499b84ac-1321-427f-aa17-267ca6975798";
        public const string clientId_vs = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";
        public static void Main(string[] args)
        {
            //============= Config [Edit these with your settings] =====================
            var vstsAccountUri = "https://mseng.VisualStudio.com";
            var restEndpoint = "_apis/projects?api-version=2.0";
            //==========================================================================

            UserData(vstsAccountUri,restEndpoint);

            //prevents console from closing immediately at the end of output
            Console.ReadLine();
        }

        static void UserData(string vstsAccountUri, string restEndpoint)
        {
            //get auth token
            AuthenticationResult result = GetToken(null);
            var authHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            //call VSTS REST API
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsAccountUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = authHeader;

                //connect to REST endpoint
                HttpResponseMessage response = client.GetAsync(restEndpoint).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\tSuccessful REST call\n");
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                }
            }
        }

        static AuthenticationResult GetToken(string tenant)
        {
            AuthenticationContext ctx = null;
            if (tenant != null)
                ctx = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            else
            {
                ctx = new AuthenticationContext("https://login.microsoftonline.com/common");
                if (ctx.TokenCache.Count > 0)
                {
                    string homeTenant = ctx.TokenCache.ReadItems().First().TenantId;
                    ctx = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
                }
            }
            AuthenticationResult result = null;
            try
            {
                result = ctx.AcquireTokenSilentAsync(resource_vsts, clientId_vs).Result;
            }
            catch (Exception exc)
            {
                var adalEx = exc.InnerException as AdalException;
                if ((adalEx != null) && (adalEx.ErrorCode == "failed_to_acquire_token_silently"))
                {
                    result = GetTokenViaCode(ctx);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something went wrong.");
                    Console.WriteLine("Message: " + exc.InnerException.Message + "\n");
                }
            }
            return result;
        }

        static AuthenticationResult GetTokenViaCode(AuthenticationContext ctx)
        {
            AuthenticationResult result = null;
            try
            {
                DeviceCodeResult codeResult = ctx.AcquireDeviceCodeAsync(resource_vsts, clientId_vs).Result;
                Console.ResetColor();
                Console.WriteLine("You need to sign in.");
                Console.WriteLine("Message: " + codeResult.Message + "\n");
                result = ctx.AcquireTokenByDeviceCodeAsync(codeResult).Result;
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong.");
                Console.WriteLine("Message: " + exc.Message + "\n");
            }
            return result;
        }
    }
}
