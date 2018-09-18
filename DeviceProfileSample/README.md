# Device Profile Sample

For a headless text output client application, it is not possible authenticate through an interactive prompt. Instead a text only approach is necessary. This flow leverages a user's external device (i.e. phone) to authenticate through an interactive login prompt and pass the auth token to the headless application. For more information [click here](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/?v=17.23h).

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses ADAL to authenticate a user via the [Device Profile flow](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/?v=17.23h) and returns a JSON string containing all account team project data viewable by the authenticated user.

To run this sample for an [Azure Active Directory](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-whatis) backed Azure DevOps account you will need:
* [Visual Studio IDE](https://www.visualstudio.com/vs/)
* An Azure Active Directory (AAD) tenant. If you do not have one, follow these [steps to set up an AAD](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-howto-tenant)
* A user account in your AAD tenant
* A Azure DevOps account backed by your AAD tenant where your user account has access. If you have an existing Azure DevOps account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your Azure DevOps account](https://www.visualstudio.com/en-us/docs/setup-admin/team-services/manage-organization-access-for-your-account-vs)

To run this sample for a [Microsoft Account](https://account.microsoft.com/account) backed Azure DevOps account you will need:
* [Visual Studio IDE](https://www.visualstudio.com/vs/)
* A Azure DevOps account not connected to AAD

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Register the sample application with you Azure Active Directory tenant (AAD backed Azure DevOps account)

```no-highlight
If you are a Microsoft Account backed Azure DevOps account please skip this step.
```

1. Sign in to the [Azure Portal](https://portal.azure.com).
2. On the top bar, click on your account and under the Directory list, choose the Active Directory tenant where you wish to register your application.
3. On the left hand navigation menu, select `Azure Active Directory`.
4. Click on `App registrations` and select `New application registration` from the top bar.
5. Enter a `name` for you application, ex. "Adal native app sample", choose `Native` for `application type`, and enter `http://adalsample` for the `Redirect URI`. Finally click `create` at the bottom of the screen.
6. Save the `Application ID` from your new application registration. You will need it later in this sample.
7. Grant permissions for Azure DevOps. Click `Required permissions` -> `add` -> `1 Select an API` -> type in and select `Azure DevOps` -> check the box for `Delegated Permissions` -> click `Select` -> click `Done` -> click `Grant Permissions` -> click `Yes`.

## Step 3: Install and configure ADAL (optional)

Package: `Microsoft.Identity.Model.Clients.ActiveDirectory` has already been installed and configured in the sample, but if you are adding to your own project you will need to [install and configure it yourself](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory). 

## Step 4a: Run the sample (AAD backed Azure DevOps account)

1. Navigate to the sample in cloned repo `vsts-auth-samples/DeviceProfileSample/`
2. Open the solution file `DeviceProfileSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
3. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `azDevOrganizationUrl` - Update this value to your VSTS collection URL, e.g. http://dev.azure.com/myaccount.
    * `clientId` - Update this value with the `Application ID` you saved in step 2.6.
5. Build and run solution. You should see a console window with instructions on how to authenticate via the Device Profile flow. After authenticating you should see all team project information viewable by the authenticated identity displayed in the console window.

## Step 4b: Run the sample (Microsoft Account backed Azure DevOps account)

1. Navigate to the sample in cloned repo `vsts-auth-samples/DeviceProfileSample/`
2. Open the solution file `DeviceProfileSample.sln` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
3. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `azDevOrganizationUrl` - Update this value to your VSTS collection URL, e.g. http://dev.azure.com/myaccount.
5. Build and run solution. You should see a console window with instructions on how to authenticate via the Device Profile flow. After authenticating you should see all team project information viewable by the authenticated identity displayed in the console window.
