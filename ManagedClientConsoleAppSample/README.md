# Managed Client ADAL Sample

The [Azure Active Directory Authentication Library (ADAL)](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-libraries) enables client application developers to authenticate cloud users and obtain access tokens for API usage.

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses ADAL to authenticate a user via an interactive prompt and return a list of all projects inside a selected VSTS account.

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Install and configure ADAL (optional)

Package: `Microsoft.Identity.Model.Clients.ActiveDirectory` has already been installed and configured in the sample, but if you are adding to your own project you will need to [install and configure it yourself](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory). 

## Step 3: Run the sample

1. Navigate to the ADAL C# sample in cloned repo `vsts-auth-samples/ManagedClientConsoleAppSample/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
3. Open the solution file `ManagedClientConsoleAppSample.csproj` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsCollectionUrl` - Mutable value. This is the url to your VSTS/TFS collection, e.g. http://myaccount.visualstudio.com for VSTS or http://myserver:8080/tfs/DefaultCollection for TFS.
5. Build and run solution. After running you should see a list of all projects inside of myaccount.




