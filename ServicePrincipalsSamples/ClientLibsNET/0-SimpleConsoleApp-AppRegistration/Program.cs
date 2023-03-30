using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;


/// PARAMETERS
const string AdoBaseUrl = "https://dev.azure.com";
const string AdoOrgName = "Your organization name";

const string AadTenantId = "Your Azure AD tenant id";
const string AadClientId = "";  // Client ID for your App Registration / Service Principal
// Set one of either clientSecret or certificateThumbprint
const string AadClientSecret = ""; // Client Secret for your App Registration / Service Principal
const string AadCertificateThumbprint = ""; // Thumbprint for your client certificate


/// CODE
TokenCredential credential;

if (!string.IsNullOrEmpty(AadClientSecret))
{
    credential = new ClientSecretCredential(AadTenantId, AadClientId, AadClientSecret);
}
else
{
    using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser); // Replace with appropriate Store Name / Location if necessary
    store.Open(OpenFlags.ReadOnly);
    var certificate = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(cert => cert.Thumbprint == AadCertificateThumbprint);

    credential = new ClientCertificateCredential(AadTenantId, AadClientId, certificate);
}

// Whenever possible, credential instance should be reused for the lifetime of the process.
// An internal token cache is used which reduces the number of outgoing calls to Azure AD to get tokens.
// Call GetTokenAsync whenever you are making a request. Token caching and refresh logic is handled by the credential object.
var tokenRequestContext = new TokenRequestContext(VssAadSettings.DefaultScopes);
var accessToken = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);

var vssAadToken = new VssAadToken("Bearer", accessToken.Token);
var vssAadCredentials = new VssAadCredential(vssAadToken);

var orgUrl = new Uri(new Uri(AdoBaseUrl), AdoOrgName);
var connection = new VssConnection(orgUrl, vssAadCredentials);


var client = connection.GetClient<WorkItemTrackingHttpClient>();

Console.Write("Work Item Command? [get or create] ");
var command = Console.ReadLine().Trim().ToLowerInvariant();
Console.WriteLine();
Console.Write("Azure DevOps Project Name? ");
var project = Console.ReadLine().Trim();
Console.WriteLine();

if (command == "get")
{
    Console.Write("Work Item ID? ");
    var workItemId = int.Parse(Console.ReadLine().Trim());
    Console.WriteLine();
    var workItem = await client.GetWorkItemAsync(project, workItemId);
    Console.WriteLine(workItem.Fields["System.Title"]);
}
else if (command == "create")
{
    Console.Write("Work Item Title? ");
    var title = Console.ReadLine().Trim();
    Console.WriteLine();
    var patchDocument = new JsonPatchDocument
    {
        new JsonPatchOperation()
        {
            Operation = Operation.Add,
            Path = "/fields/System.Title",
            Value = title
        }
    };

    try
    {

        var result = await client.CreateWorkItemAsync(patchDocument, project, "bug");
        Console.WriteLine($"Work item created: Id = {result.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return -1;
    }
}


return 0;