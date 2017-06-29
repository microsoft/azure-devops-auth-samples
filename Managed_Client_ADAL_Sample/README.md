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

1. Navigate to the ADAL C# sample in cloned repo `vsts-auth-samples/Managed_Client_Sample_(ADAL_C#)/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
2. Open the solution file `SimpleAdalConsoleApp.sln` in [Visual Studio IDE](https://www.visualstudio.com/downloads/)
3. Open CS file `Program.cs` and there will be 3 input fields:
    * `VSTSResourceId` - Immutable value. Denotes that we need a VSTS access token.
    * `clientId` - Immutable value*. *Normally your app's registered AAD clientId, but for VSTS is must be the VS client ID provided
    * `VSTSAccountName` - Mutable value. Update with the name of the VSTS account you would like to access. (e.g. "myaccount" from myaccount.visualstuido.com)
4. Build and run solution. After running you should see a list of all projects inside of myaccount.




