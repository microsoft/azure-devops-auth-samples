# Non-interactive PAT generation sample

This sample shows how to generate a Personal Access Token (PAT) using the [Client Libraries](https://learn.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops) and the [PAT Lifecycle Management API](https://learn.microsoft.com/en-us/rest/api/azure/devops/tokens). Requests to this API need to be authorized with an Azure Active Directory (AAD) access token. 

This sample uses the `PublicClientApplicationBuilder` from **Microsoft Authentication Library (MSAL)** rather than relying on the interactive pop-up dialog to get an AAD access token which is then used as a credential to authenticate requests to Azure DevOps. This is meant to be used in scenarios where you need to generate a PAT associated with an account that does not have interactive login rights.

## How to run this sample

**Prerequisites**

- [.NET Framework 4.7.2 SDK](https://dotnet.microsoft.com/en-us/download/dotnet-framework)
- [An Application in your Azure Active Directory (Azure AD) tenant](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- _(optional)_ [Visual Studio / Visual Studio Code](https://visualstudio.microsoft.com/downloads/)


### Step 1: Clone or download this repository

From a shell or command line:
```
git clone https://github.com/microsoft/azure-devops-auth-samples.git
```

### Step 2: Configure the sample to use your Azure AD application

Update the configuration file `App.config` with the information about your AAD application, AAD credentials, and Azure DevOps organization.

### Step 3: Run the sample

**From Visual Studio:**
1. Open the solution file `NonInteractivePatGenerationSample.sln`.
2. Build and run the project.

-- OR --

**From the command line:**
```cmd
cd NonInteractivePatGenerationSample/NonInteractivePatGenerationSample
dotnet run
```
