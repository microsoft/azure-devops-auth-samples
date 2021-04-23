# Device Profile Sample

For a headless text output client application, it is not possible authenticate through an interactive prompt. Instead a text only approach is necessary. This flow leverages a user's external device (i.e. phone) to authenticate through an interactive login prompt and pass the auth token to the headless application. For more information [click here](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-device-code).

If the tenant admin requires device authentication conditional access policies, using the Device profile flow won't be a good option.

## Sample Application

This sample will walk you through the steps to create a client-side console application which uses **MSAL.NET** to authenticate a user via the [Device Profile flow](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-device-code) and returns a JSON string containing all account team project data viewable by the authenticated user.

To run this sample you will need:

- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- An **Azure AD** tenant. For more information see: [How to get an Azure AD tenant](https://docs.microsoft.com/azure/active-directory/develop/quickstart-create-new-tenant)
- A user account in your **Azure AD** tenant.
- A Azure DevOps account backed by your AAD tenant where your user account has access. If you have an existing Azure DevOps account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your Azure DevOps account](https://docs.microsoft.com/azure/devops/organizations/accounts/manage-azure-active-directory-groups-vsts?view=vsts&tabs=new-nav)

To run this sample for a [Microsoft Account](https://account.microsoft.com/account) backed Azure DevOps account you will need:

- Azure DevOps account not connected to AAD.

## Step 1: Clone or download this repository

From a shell or command line:

```console
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Register the sample application with you Azure Active Directory tenant (AAD backed Azure DevOps account)

```no-highlight
If you are a Microsoft Account backed Azure DevOps account please skip this step.
```

1. Navigate to the Microsoft identity platform for developers [App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page.
1. Select **New registration**.
1. In the **Register an application page** that appears, enter your application's registration information:
   - In the **Name** section, enter a meaningful application name that will be displayed to users of the app, for example `MSAL-DeviceCodeFlow`.
   - Under **Supported account types**, select **Accounts in this organizational directory only**.
1. Select **Register** to create the application.
1. In the app's registration screen, find and note the **Application (client) ID**. You use this value in your app's configuration file(s) later in your code.
   - In the **Advanced settings** | **Default client type** section, flip the switch for `Treat application as a public client` to **Yes**.
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

1. Open the `DeviceProfileSample\App.config` file.
1. Find the key `ida:ClientID` and replace the existing value with the application ID (clientId) of `ManagedClientConsoleAppSample` app copied from the Azure portal.
1. Find the key `ida:Tenant` and replace the existing value with your Azure AD tenant ID or tenant domain.
1. Find the key `ado:OrganizationUrl` and replace the existing value to the URL of your Azure DevOps organization; NOTE: This must use HTTPS.

## Running the sample

Clean the solution, rebuild the solution, and run it.

Use a web browser to open the Url (https://microsoft.com/devicelogin) that is displayed in console app. Input the code presented in the console , sign-in and check the result of the operation back in the console.
