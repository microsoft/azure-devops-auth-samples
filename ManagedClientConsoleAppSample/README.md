# Managed Client ADAL Sample

For native applications which can support interative authentication prompts, the [Azure Active Directory Authentication Library (ADAL)](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-libraries) makes it easy to setup authentication flows for users.

For native applications which cannot support interactive authentication prompts, please check out our [Device Profile Sample](./../DeviceProfileSample/README.md).

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses ADAL to authenticate a user via an interactive prompt and return a list of all projects inside a selected VSTS account.

To run this sample for an [Azure Active Directory](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-whatis) backed VSTS account you will need:
* [Visual Studio IDE](https://www.visualstudio.com/vs/)
* An Azure Active Directory (AAD) tenant. If you do not have one, follow these [steps to set up an AAD](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-howto-tenant)
* A user account in your AAD tenant
* A VSTS account backed by your AAD tenant where your user account has access. If you have an existing VSTS account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your VSTS account](https://www.visualstudio.com/en-us/docs/setup-admin/team-services/manage-organization-access-for-your-account-vs)

To run this sample for a [Microsoft Account](https://account.microsoft.com/account) backed VSTS account you will need:
* [Visual Studio IDE](https://www.visualstudio.com/vs/)
* A VSTS account not connected to AAD


## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Register the sample application with you Azure Active Directory tenant (AAD backed VSTS account)

```no-highlight
If you are a Microsoft Account backed VSTS account please skip this step.
```

1. Sign in to the [Azure Portal](https://portal.azure.com).
2. On the top bar, click on your account and under the Directory list, choose the Active Directory tenant where you wish to register your application.
3. On the left hand navigation menu, select `Azure Active Directory`.
4. Click on `App registrations` and select `New application registration` from the top bar.
5. Enter a `name` for you application, ex. "Adal native app sample", choose `Native` for `application type`, and enter `http://adalsample` for the `Redirect URI`. Finally click `create` at the bottom of the screen.
6. Save the `Application ID` from your new application registration. You will need it later in this sample.
7. Grant permissions for VSTS. Click `Required permissions` -> `add` -> `1 Select an API` -> type in and select `Microsoft Visual Studio Team Services` -> check the box for `Delegated Permissions` -> click `Select` -> click `Done` -> click `Grant Permissions` -> click `Yes`.

## Step 3: Install and configure ADAL (optional)

Package: `Microsoft.Identity.Model.Clients.ActiveDirectory` has already been installed and configured in the sample, but if you are adding to your own project you will need to [install and configure it yourself](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory). 

## Step 4a: Run the sample (AAD backed VSTS account)

1. Navigate to the ADAL C# sample in cloned repo `vsts-auth-samples/ManagedClientConsoleAppSample/`.
2. Open the solution file `ManagedClientConsoleAppSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/).
3. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed.
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsCollectionUrl` - update this with the url to your VSTS/TFS collection, e.g. http://myaccount.visualstudio.com for VSTS.
    * `clientId` - update this with the `application id` you saved from step 2.6 above.
    * `replyUri` - update this to `http://adalsample`, you can add other reply urls in [azure portal](https://portal.azure.com)
5. Build and run the solution. After running you should see an interactive login prompt. Then after authentication and authorization, a list of all projects inside of your account.

## Step 4b: Run the sample (Microsoft Account backed VSTS account)

1. Navigate to the ADAL C# sample in cloned repo `vsts-auth-samples/ManagedClientConsoleAppSample/`.
2. Open the solution file `ManagedClientConsoleAppSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/).
3. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed.
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsCollectionUrl` - update this with the url to your VSTS/TFS collection, e.g. http://myaccount.visualstudio.com for VSTS.
    * `clientId` - Do not change this value. It must be used to run the sample successfully.
    * `replyUri` - Do not change this value. It must be used to run the sample successfully.
5. Build and run the solution. After running you should see an interactive login prompt. Then after authentication and authorization, a list of all projects inside of your account.

