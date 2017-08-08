# Managed Client ADAL Sample

The [Azure Active Directory Authentication Library (ADAL)](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-libraries) enables client application developers to authenticate cloud users and obtain access tokens for API usage.

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses ADAL to authenticate a user via an interactive prompt and return a list of all projects inside a selected VSTS account.

To run this sample you will need:
* Visual Studio 2017
* An Azure Active Directory (AAD) tenant. If you do not have one, follow these [steps to set up an AAD](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-howto-tenant)
* A user account in your AAD tenant
* A VSTS account backed by your AAD tenant where your user account has access. If you have an existing VSTS account not connected to your AAD tenant follow these [steps to connect you AAD tenant to your VSTS account](https://www.visualstudio.com/en-us/docs/setup-admin/team-services/manage-organization-access-for-your-account-vs)

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Register the sample application with you Azure Active Directory tenant

1. Sign in to the [Azure Portal](https://portal.azure.com).
2. On the top bar, click on your account and under the Directory list, choose the Active Directory tenant where you wish to register your application.
3. On the left hand navigation menu, select `Azure Active Directory`.
4. Click on `App registrations` and select `New application regirstation` from the top bar.
5. Enter a `name` for you application, ex. "Adal native app sample", choose `Native` for `application type`, and enter `http://localhost:8080` for the `Redirect URI`. Finally click `create` at the bottom of the screen.
6. Save the `Application ID` from your new application registration. You will need it later in this sample.
7. Grant Permissions for VSTS. Click `Required permissions` -> `add` -> `1 Select an API` -> type in and select `Microsoft Visual Studio Team Services` -> check the box for `Have full access to...` -> click `Save` -> click `Grant Permissions` -> click `Yes`.

## Step 3: Install and configure ADAL (optional)

Package: `Microsoft.Identity.Model.Clients.ActiveDirectory` has already been installed and configured in the sample, but if you are adding to your own project you will need to [install and configure it yourself](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory). 

## Step 4: Run the sample

1. Navigate to the ADAL C# sample in cloned repo `vsts-auth-samples/ManagedClientConsoleAppSample/`.
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed.
3. Open the solution file `ManagedClientConsoleAppSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/).
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsCollectionUrl` - update this with the url to your VSTS/TFS collection, e.g. http://myaccount.visualstudio.com for VSTS or http://myserver:8080/tfs/DefaultCollection for TFS.
    * `clientId` - update this with the `application id` you saved from `portal.azure.com`
    * `replyUri` - we have set this to `http://localhost:8080`, but please update it as necessary for your own app.
5. Build and run solution. After running you should see an interactive login prompt. Then after authentication and authorization, a list of all projects inside of your account.




