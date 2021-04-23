# Managed Client MSAL Sample

For native applications which can support interative authentication prompts, the [Microsoft Authentication Library (MSAL)](https://docs.microsoft.com/azure/active-directory/develop/msal-overview) makes it easy to setup authentication flows for users.

For native applications which cannot support interactive authentication prompts, please check out our [Device Profile Sample](./../DeviceProfileSample/README.md).

## Sample Application

This sample will walk you through the steps to create a client-side console application which uses **MSAL.NET** to authenticate a user via an interactive prompt and return a list of all projects inside a selected Azure DevOps account.

To run this sample you will need:

- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- An **Azure AD** tenant. For more information see: [How to get an Azure AD tenant](https://docs.microsoft.com/azure/active-directory/develop/quickstart-create-new-tenant)
- A user account in your **Azure AD** tenant.
- A Azure DevOps account backed by your AAD tenant where your user account has access. If you have an existing Azure DevOps account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your Azure DevOps account](https://docs.microsoft.com/azure/devops/organizations/accounts/manage-azure-active-directory-groups-vsts?view=vsts&tabs=new-nav)

To run this sample for a [Microsoft Account](https://account.microsoft.com/account) backed Azure DevOps account you will need:

- Azure DevOps account not connected to AAD.

## Step 1: Clone or download this repository

From your shell or command line:

```console
git clone https://github.com/microsoft/azure-devops-auth-samples.git
```

## Step 2: Register the application

1. Navigate to the [Azure portal](https://portal.azure.com) and select the **Azure AD** service.
1. Select the **App Registrations** blade on the left, then select **New registration**.
1. In the **Register an application page** that appears, enter your application's registration information:
   - In the **Name** section, enter a meaningful application name that will be displayed to users of the app, for example `ManagedClientConsoleAppSample`.
   - Under **Supported account types**, select **Accounts in this organizational directory only**.
1. Select **Register** to create the application.
1. In the app's registration screen, find and note the **Application (client) ID**. You use this value in your app's configuration file(s) later in your code.
1. In the app's registration screen, select **Authentication** in the menu.
   - If you don't have a platform added, select **Add a platform** and select the **Public client (mobile & desktop)** option.
   - In the **Redirect URIs** | **Suggested Redirect URIs for public clients (mobile, desktop)** section, select **https://login.microsoftonline.com/common/oauth2/nativeclient**
1. Select **Save** to save your changes.
1. In the app's registration screen, select the **API permissions** blade in the left to open the page where we add access to the APIs that your application needs.
   - Select the **Add a permission** button and then,
   - Ensure that the **Microsoft APIs** tab is selected.
   - In the list of APIs, select the API `Azure DevOps`.
   - In the **Delegated permissions** section, select the **user_impersonation** in the list. Use the search box if necessary.
   - Select the **Add permissions** button at the bottom.

## Step 3: Configure the application to use your app registration

Open the project in your IDE (like Visual Studio or Visual Studio Code) to configure the code.

> In the steps below, "ClientID" is the same as "Application ID" or "AppId".

1. Open the `ManagedClientConsoleAppSample\App.config` file.
1. Find the key `ida:ClientID` and replace the existing value with the application ID (clientId) of `ManagedClientConsoleAppSample` app copied from the Azure portal.
1. Find the key `ida:Tenant` and replace the existing value with your Azure AD tenant ID or tenant domain.
1. Find the key `ado:OrganizationUrl` and replace the existing value to the URL of your Azure DevOps organization; NOTE: This must use HTTPS.

## Running the sample

> For Visual Studio Users
>
> Clean the solution, rebuild the solution, and run it.  You might want to go into the solution properties and set both projects as startup projects, with the service project starting first.

## Explore the sample

1. Sign-in when you see an interactive login prompt.
1. In console, output will be the list of projects in the organization that the authenticated user has access to.
