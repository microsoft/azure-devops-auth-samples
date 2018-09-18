# Dual Support (Azure DevOps/TFS) Client Sample

For windows native applications which want to target both Azure DevOps and TFS we recommend using the [Client Libraries for Azure DevOps and TFS](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=vsts) to generate interactive sign in prompts for Azure DevOps users and leverage seemless Windows credential authentication for TFS users.

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses Client Libraries - Interactive and Windows Auth to authenticate a Azure DevOps or TFS user and return a list of all projects inside a selected Azure DevOps account or TFS collection.

To run this sample you will need:
* [Visual Studio IDE](https://www.visualstudio.com/vs/)
* A [Azure DevOps account](https://www.visualstudio.com/team-services/)

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Install and configure Client libraries (optional)

Packages: [Microsoft.VisualStudio.Services.Client](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.Client), and [Microsoft.TeamFoundation.Core.WebApi](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client) have already been installed and configured in the sample, but if you are adding to your own project you will need to install and configure it yourself.

## Step 3: Run the sample

1. Navigate to the ADAL C# sample in cloned repo `vsts-auth-samples/DualSupportClientSample/`.
2. Open the solution file `VstsTfsSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/).
3. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed.
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsOrTfsCollectionUrl` - update this with the url to your Azure DevOps/TFS collection, e.g. http://dev.azure.com/myaccount for Azure DevOps or http://myserver:8080/tfs/DefaultCollection for TFS.
5. Build and run the solution. After running you should see an interactive login prompt if you are a Azure DevOps user. If you are a TFS user authentication should happen in the background. After authentication and authorization, a list of all projects inside of your account will be displayed in the console.

