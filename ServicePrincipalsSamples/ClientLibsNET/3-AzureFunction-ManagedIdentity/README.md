# Azure Function using an Azure AD Managed Identity to get a work item

This sample shows how to get an Azure AD access token for a Managed Identity using [Azure Identity client library for .NET](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet) and authenticate to Azure DevOps to create or get a work item.

## How to run this sample

**Prerequisites**

- [.NET Core SDK](https://dotnet.microsoft.com/en-us/download) - 6.0 or higher
- [Azure DevOps .NET client libraries](https://learn.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops) - 19.219.0-preview or higher
- [Visual Studio / Visual Studio Code](https://aka.ms/vsdownload)

### Step 1: Clone or download this repository

From a shell or command line: 

```ps
git clone https://github.com/microsoft/azure-devops-auth-samples.git
```

### Step 2: Create an Azure Function with a Managed Identity assigned

1. To create an Azure Function, see [Create your first function in the Azure portal](https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal).
2. To assign it a Managed Identiy, see [How to use managed identities for App Service and Azure Functions](https://learn.microsoft.com/en-us/azure/app-service/overview-managed-identity?tabs=portal%2Chttp).

### Step 3: Add the Managed Identity to your Azure DevOps Organization

Once the Managed Identity is created, [add it to your Azure DevOps organization](https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/service-principal-managed-identity#step-by-step-configuration).

### Step 2: Configure the sample

Update constants in the file `TestMIHttpTrigger.cs` with the information about your Azure AD Managed Identity and Azure DevOps organization.

```cs
public const string AdoOrgName = "Your organization name";

public const string AadTenantId = "Your Azure AD tenant id";
// ClientId for User Assigned Managed Identity. Leave null for System Assigned Managed Identity
public const string AadUserAssignedManagedIdentityClientId = null;
```

### Step 5: Run the sample

The sample will use different credentials depending on the environment.

- **In Azure**, the managed identity will be used.

**Test in dev environment**

In Visual Studio:

1. Open the solution file `../ServicePrincipalsSamples.sln`.
2. Configure the Azure account to be used in `Tools -> Options -> Azure Service Authentication -> Account Selection`.
3. Build the project and run using the profile `AzureFunctionTest`.
4. In the output you will get the function URL that you can call with the query parameter `?workItemId={put here some work item id}`.

See [Azure Identity client library for .NET](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#defaultazurecredential) for more details and options for providing local development credentials.

**Run in the Azure VM**

Follow the following steps from the guide [Quickstart: Create your first C# function in Azure using Visual Studio](https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio?tabs=in-process#publish-the-project-to-azure):

1. Publish the project to Azure
2. Verify your function in Azure
    
# References 

- [Introduction to Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview)
- [Azure.Identity - ManagedIdentityCredential.GetTokenAsync Method](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential.gettokenasync?view=azure-dotnet)