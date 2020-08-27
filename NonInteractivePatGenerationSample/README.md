# Non-interactive PAT generation Sample

This sample shows how to generate a Personal Access Token (PAT) using the [Client Libraries](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=vsts). You cannot use a PAT to generate a PAT and so this sample makes use of the Azure Active Directory Authentication Library (ADAL) to acquire an AAD token which is then used as a credential to authenticate requests to Azure DevOps. This sample also uses the UserPasswordCredential rather than relying on the interactive pop-up dialog - this can be useful in scenarios where you need to generate a PAT associated with an account that does not have interactive login rights.

## Sample Application

This sample shows the basic flow for calling ADAL, and then using the resulting token as credentials to call the ```CreateSessionTokenAsync``` method on the ```TokenHttpClient``` class. In order to run this application you will need to [register your own AAD application](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v1-integrate-apps-with-azure-ad).

 ## Step 1: Clone or download azure-devops-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/microsoft/azure-devops-auth-samples.git 
```

## Step 2: Run the sample

1. Navigate to the sample in cloned repo `azure-devops-auth-samples/NonInteractivePatGenerationSample/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
3. Open the solution file `NonInteractivePatGenerationSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
4. Open CS file ```Program.cs``` and look at the first few lines in the ```Main``` method. Replace the username and password with the appropriate values. You will need to get the ```aadApplicationID``` value by [registering your app in AAD](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v1-integrate-apps-with-azure-ad).
6. Build and run the solution, the generate PAT will be output to the console window.
