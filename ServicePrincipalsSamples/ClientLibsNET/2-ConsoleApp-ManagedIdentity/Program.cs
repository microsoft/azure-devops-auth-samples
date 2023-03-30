using Azure.Core;
using Azure.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http.Headers;

namespace ServicePrincipalsSamples
{
    public static class Program
    {
        public const string AdoBaseUrl = "https://dev.azure.com";

        public const string AdoOrgName = "Your organization name";

        public const string AadTenantId = "Your Azure AD tenant id";
        // ClientId for User Assigned Managed Identity. Leave null for System Assigned Managed Identity
        public const string AadUserAssignedManagedIdentityClientId = null;

        public static List<ProductInfoHeaderValue> AppUserAgent { get; } = new()
        {
            new ProductInfoHeaderValue("Identity.ManagedIdentitySamples", "1.0"),
            new ProductInfoHeaderValue("(2-ConsoleApp-ManagedIdentity)")
        };

        public static async Task Main()
        {
            Console.Write("Work item ID: ");
            int workItemId = Convert.ToInt32(Console.ReadLine());

            var vssConnection = await CreateVssConnection();

            var workItemTrackingHttpClient = vssConnection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await workItemTrackingHttpClient.GetWorkItemAsync(workItemId);

            Console.WriteLine($"Work Item Title: {workItem.Fields["System.Title"]}");
        }

        private static async Task<VssConnection> CreateVssConnection()
        {
            var accessToken = await GetManagedIdentityAccessToken();
            var token = new VssAadToken("Bearer", accessToken);
            var credentials = new VssAadCredential(token);

            var settings = VssClientHttpRequestSettings.Default.Clone();
            settings.UserAgent = AppUserAgent;

            var organizationUrl = new Uri(new Uri(AdoBaseUrl), AdoOrgName);
            return new VssConnection(organizationUrl, credentials, settings);
        }

        private static async Task<string> GetManagedIdentityAccessToken()
        {
            // DefaultAzureCredential will use VisualStudioCredentials or other appropriate credentials for local development
            // but will use ManagedIdentityCredential when deployed to an Azure Host with Managed Identity enabled.
            // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#defaultazurecredential
            var credential =
                new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions
                    {
                        TenantId = AadTenantId,
                        ManagedIdentityClientId = AadUserAssignedManagedIdentityClientId,
                        ExcludeEnvironmentCredential = true // Excluding because EnvironmentCredential was not using correct identity when running in Visual Studio
                    });

            var tokenRequestContext = new TokenRequestContext(VssAadSettings.DefaultScopes);
            var token = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);

            return token.Token;
        }
    }
}