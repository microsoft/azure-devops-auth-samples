# Azure AD Service Principals and Managed Identities in Azure DevOps (.NET Core)

These .NET Core samples show how to use Azure AD Service Principals and Managed Identities to authenticate to Azure DevOps using [Microsoft Authentication Library for .NET (MSAL.NET)](https://aka.ms/msal-net) and the [Azure DevOps .NET client libraries](https://learn.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops).

## Code samples

| Project | Description |
|--|--|
| [0-SimpleConsoleApp-AppRegistration](/ServicePrincipalsSamples/ClientLibsNET/0-SimpleConsoleApp-AppRegistration/) | It uses an Azure AD Application Service Principal to create get a work item. |
| [1-ConsoleApp-AppRegistration](/ServicePrincipalsSamples/ClientLibsNET/1-ConsoleApp-AppRegistration/) | It uses an Azure AD Application Service Principal to perform multiple operations in Azure DevOps. It also shows how to use the MSAL in-memory token cache and handle the access token expiration. |
| [2-ConsoleApp-ManagedIdentity](/ServicePrincipalsSamples/ClientLibsNET/2-ConsoleApp-ManagedIdentity/) | It uses an Azure AD Managed Identity to get a work item. |
| [3-AzureFunction-ManagedIdentity](/ServicePrincipalsSamples/ClientLibsNET/3-AzureFunction-ManagedIdentity/) | It uses an Azure AD Managed Identity to get a work item using an Azure Function. |

## Usage with Azure DevOps .NET client libs

Learn how to use Azure AD Service Principals with the client libs.

### How to replace an Azure DevOps PAT with an Azure AD access token

Creating Azure DevOps credentials is very similar in both cases:

```cs
// Azure DevOps PAT
var credentials = new VssBasicCredential(string.Empty, "pat_secret");

// Azure AD Service Principal access token
var token = new VssAadToken("Bearer", "aad_access_token");
var credentials = new VssAadCredential(token);
```

Then any of them can be used to create an instance of `VssConnection` (remember that this instance should be a singleton in the application):

```cs
var organizationUrl = "http://dev.azure.com/Fabrikam";
var vssConnection = new VssConnection(organizationUrl, credentials);
var adoClient = vssConnection.GetClient<_AdoHttpClient_>();
```

### How to regenerate an Azure AD access token using VssConnection

As Azure AD access tokens are short-lived, you can provide a delegate to `VssAadToken` to acquire a new access token when the existing one expires. [Microsoft Authentication Library for .NET (MSAL.NET)](https://aka.ms/msal-net-authenticationresult) has a token cache and handles automatically the token acquisition when an access token is expired.

For example, using an Azure AD application with a client secret:

> **Note:** At the moment of writing this, `VssAadToken` does not support asynchronous delegates

```cs
var app = ConfidentialClientApplicationBuilder.Create("client_id")
    .WithClientSecret("client_secret")
    .WithAuthority("https://login.microsoftonline.com/tenant_id")
    .Build();

// It uses Azure DevOps default scope (499b84ac-1321-427f-aa17-267ca6975798/.default)
var token = new VssAadToken((scopes) => app.AcquireTokenForClient(scopes).ExecuteAsync().SyncResultConfigured());
var credentials = new VssAadCredential(token);
```

See [1-ConsoleApp-AppRegistration](/ServicePrincipalsSamples/ClientLibsNET/1-ConsoleApp-AppRegistration/) for more information.
