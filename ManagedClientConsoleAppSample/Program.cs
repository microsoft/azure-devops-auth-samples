using Microsoft.Identity.Client;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ManagedClientConsoleAppSample
{
    class Program
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The Tenant is the name or Id of the Azure AD tenant in which this application is registered.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        //
        internal static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        internal static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        internal static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        internal static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        //URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = ConfigurationManager.AppSettings["ado:OrganizationUrl"];

        internal static string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" }; //Constant value to target Azure DevOps. Do not change  
        
        // MSAL Public client app
        private static IPublicClientApplication application;

        public static async Task Main(string[] args)
        {
            var accessToken = await SignInUserAndGetTokenUsingMSAL(scopes);

            var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", accessToken);
            
            ListProjects(bearerAuthHeader);

            Console.ReadLine();
        }

        /// <summary>
        /// Sign-in user using MSAL and obtain an access token for Azure DevOps
        /// </summary>
        /// <param name="scopes"></param>
        /// <returns>Access Token</returns>
        private static async Task<string> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(clientId)
                                       .WithAuthority(authority)
                                       .WithDefaultRedirectUri()
                                       .Build();
            
            AuthenticationResult result = null;

            try
            {
                var accounts = application.GetAccountsAsync().Result;
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // If the token has expired, prompt the user with a login prompt
                result = await application.AcquireTokenInteractive(scopes)
                        .WithClaims(ex.Claims)
                        .ExecuteAsync();
            }
            return result.AccessToken;
        }

        /// <summary>
        /// Get all projects in the organization that the authenticated user has access to and print the results.
        /// </summary>
        /// <param name="authHeader"></param>
        private static void ListProjects(AuthenticationHeaderValue authHeader)
        {
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
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
                    Console.WriteLine("Succesful REST call");
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
