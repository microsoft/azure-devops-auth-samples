using Microsoft.Identity.Client;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DeviceProfileSample
{
    public class Program
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

        //Constant value to target Azure DevOps. Do not change  
        internal static string[] scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" };
        
        //URL of your Azure DevOps account.
        internal static string azureDevOpsOrganizationUrl = ConfigurationManager.AppSettings["ado:OrganizationUrl"];
        
        // The MSAL Public client app
        private static IPublicClientApplication application;

        public static async Task Main(string[] args)
        {

            AuthenticationResult result = null;
            try
            {

                result = await SignInUserAndGetTokenUsingMSAL(scopes);

                var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                ListProjects(bearerAuthHeader);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong.");
                Console.WriteLine("Message: " + ex.Message + "\n");
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Signs in the user using the device code flow and obtains an Access token for MS Graph
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        private static async Task<AuthenticationResult> SignInUserAndGetTokenUsingMSAL(string[] scopes)
        {
            // Initialize the MSAL library by building a public client application
            application = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithDefaultRedirectUri()
                                                    .Build();


            AuthenticationResult result;

            try
            {
                var accounts = await application.GetAccountsAsync();
                // Try to acquire an access token from the cache. If device code is required, Exception will be thrown.
                result = await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                result = await application.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
                {
                    // This will print the message on the console which tells the user where to go sign-in using
                    // a separate browser and the code to enter once they sign in.
                    // The AcquireTokenWithDeviceCode() method will poll the server after firing this
                    // device code callback to look for the successful login of the user via that browser.
                    // This background polling (whose interval and timeout data is also provided as fields in the
                    // deviceCodeCallback class) will occur until:
                    // * The user has successfully logged in via browser and entered the proper code
                    // * The timeout specified by the server for the lifetime of this code (typically ~15 minutes) has been reached
                    // * The developing application calls the Cancel() method on a CancellationToken sent into the method.
                    //   If this occurs, an OperationCanceledException will be thrown (see catch below for more details).
                    Console.WriteLine(deviceCodeResult.Message);
                    return Task.FromResult(0);
                }).ExecuteAsync();
            }
            return result;
        }

        private static void ListProjects(AuthenticationHeaderValue authHeader)
        {
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(azureDevOpsOrganizationUrl);
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
