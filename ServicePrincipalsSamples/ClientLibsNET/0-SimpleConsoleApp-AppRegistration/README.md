# Simple .NET Core console application using an Azure AD Application to create/get work items

This sample shows how to get an Azure AD access token for an Application Service Principal (using a secret or a certificate) using [Azure Identity client library for .NET](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet) and authenticate to Azure DevOps to create or get a work item.

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

### Step 2: Create an Azure AD application

In the tenant to which your Azure DevOps organization is connected, [create an Azure AD Application](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app).

### Step 3: Add the Azure AD application to your Azure DevOps Organization

Once the application is created, [add it to your Azure DevOps organization](https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/service-principal-managed-identity#step-by-step-configuration).

### Step 4: Configure the sample to use your Azure AD application

Update parameters in the file `Program.cs` with the information about your Azure AD application and Azure DevOps organization.

```cs
/// PARAMETERS
const string orgName = "YOUR ORG NAME";

const string tenantId = "YOUR TENANT ID";
const string clientId = "";  // Client ID for your App Registration / Service Principal
// Set one of either clientSecret or certificateThumbprint
const string clientSecret = ""; // Client Secret for your App Registration / Service Principal
const string certificateThumbprint = ""; // Thumbprint for your client certificate
```

### Step 5: Run the sample

From the console:

```cmd
cd 0-SimpleConsoleApp-AppRegistration
dotnet run
```

# References 

- [Azure.Identity - ClientCertificateCredentials](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.clientcertificatecredential?view=azure-dotnet)
- [Azure.Identity - ClientSecretCredentials](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.clientsecretcredential?view=azure-dotnet)