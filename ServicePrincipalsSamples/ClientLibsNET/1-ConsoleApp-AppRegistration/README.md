# .NET Core console application using an Azure AD Application to call Azure DevOps REST API

This sample shows how to get an Azure AD access token for an Application Service Principal (using a secret or a certificate) using [Microsoft Authentication Library for .NET (MSAL.NET)](https://aka.ms/msal-net) and authenticate to Azure DevOps to perform multiple operations. It covers the following topics:

- Usage comparison between Azure AD Service Principal access tokens and Azure DevOps PATs.
- How to use `VssConnection` following best practises.
- How to use the MSAL in-memory token cache and handle the access token expiration.

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

Update the configuration file `Settings\appsettings.json` with the information about your Azure AD application and Azure DevOps organization.

### Step 5: Run the sample

**From Visual Studio:**
1. Open the solution file `../ServicePrincipalsSamples.sln`.
2. Build the project and run using the profile `ConsoleApp (Default)`.

-- OR --

**From the console:**
```cmd
cd 1-ConsoleApp-AppRegistration
dotnet run
```

## App settings profiles

You can switch between settings files by using profiles. For example, to use a profile called `Dev`:

1. Create a copy of `appsettings.json` and name it `appsettings.Dev.json`. In this sample, non-default profile settings are excluded from Git via `.gitignore`.
2. Run the app:
   - In Visual Studio, run using the profile `ConsoleApp (Dev)` (already configured in this sample). To use a new one you need to add it to `Properties\launchSettings.json`.
   - In the console:
        ```cmd
        cd 1-ConsoleApp-AppRegistration
        dotnet run --environment Dev
        ```

## References

- [Authentication flow support in MSAL](https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-authentication-flows#client-credentials)
- [Acquire and cache tokens using the Microsoft Authentication Library (MSAL)](https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-acquire-cache-tokens)
- [A .NET Core daemon console application using MSAL.NET to acquire tokens for resources](https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2)
- [Use IHttpClientFactory to implement resilient HTTP requests](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)