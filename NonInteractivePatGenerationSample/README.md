# Non-interactive PAT generation Sample

This sample shows how to generate a Personal Access Token (PAT) using the [Client Libraries](https://www.visualstudio.com/en-us/docs/integrate/get-started/client-libraries/dotnet). You cannot use a PAT to generate a PAT and so this sample makes use of the Azure Active Directory Authentication Library (ADAL) to acquire an AAD token which is then used as a credential to authenticate requests to VSTS. This sample also uses the UserPasswordCredential rather than relying on the interactive pop-up dialog - this can be useful in scenarios where you need to generate a PAT associated with an account that does not have interactive login rights.

## Sample Application

This sample shows the basic flow for calling ADAL, and then using the resulting token as credentials to call the ```CreateSessionToken``` method on the ```DelegatedAuthorizationHttpClient``` class. In order to run this application you will need to [register your own AAD application](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-integrating-applications).

 ## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Run the sample

1. Navigate to the sample in cloned repo `vsts-auth-samples/NonInteractivePatGenerationSample/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
3. Open the solution file `NonInteractivePatGenerationSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
4. Open CS file `Program.cs` and look for the ```[value]``` placeholders.
5. Replace them with the appropriate values.
6. Build and run the solution, the generate PAT will be output to the console window.