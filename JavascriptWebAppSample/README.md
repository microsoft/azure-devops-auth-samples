# Javascript Web App Sample

For javascript web applications that want access to resources like Azure DevOps REST API, they will have to support an authentication flow for their users. The [Microsoft Authentication Library for JavaScript](https://github.com/AzureAD/microsoft-authentication-library-for-js) (MSAL.js) enables javascript application developers to setup interactive authentication flows and obtain access tokens for API usage.

## Sample Application

This sample will walk you through the steps to create a single-page javascript application which uses **MSAL.js** to authenticate a user via an interactive prompt and display all projects contained in an Azure DevOps account/TFS collection.

To run this sample you will need:

- [Node.js](https://nodejs.org/en/download/) must be installed to run this sample.
- A modern web browser. This sample uses **ES6** conventions and will not run on **Internet Explorer**.
- [Visual Studio Code](https://code.visualstudio.com/download) is recommended for running and editing this sample.
- An Azure Active Directory (Azure AD) tenant. For more information on how to get an Azure AD tenant, see: [How to get an Azure AD tenant](https://azure.microsoft.com/documentation/articles/active-directory-howto-tenant/)
- A user account in your Azure AD tenant.
- An Azure DevOps account backed by your AAD tenant where your user account has access. If you have an existing Azure DevOps account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your Azure DevOps account](https://docs.microsoft.com/azure/devops/organizations/accounts/manage-azure-active-directory-groups-vsts?view=vsts&tabs=new-nav)

## Step 1: Clone or download azure-devops-auth-samples repository

From a shell or command line:

```console
git clone https://github.com/microsoft/azure-devops-auth-samples.git
```

Then locate the sample folder, where `package.json` file resides. Once you do, type:

```console
npm install
```

## Step 2: Register the app

1. Navigate to the Microsoft identity platform for developers [App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page.
1. Select **New registration**.
1. In the **Register an application page** that appears, enter your application's registration information:
   - In the **Name** section, enter a meaningful application name that will be displayed to users of the app, for example `devops-js-app`.
   - Under **Supported account types**, select **Accounts in your organizational directory only**.
   - In the **Redirect URI (optional)** section, select **Single-Page Application** in the combo-box and enter the following redirect URI: `http://localhost:8081/`.
1. Select **Register** to create the application.
1. In the app's registration screen, find and note the **Application (client) ID**. You use this value in your app's configuration file(s) later in your code.
1. Select **Save** to save your changes.

## Step 3: Configure the app to use your app registration

Open the project in your IDE (like Visual Studio or Visual Studio Code) to configure the code.

> In the steps below, "ClientID" is the same as "Application ID" or "AppId".

1. Open the `config.js` file
1. Find the key `Enter_the_Application_Id_Here` and replace the existing value with the application ID (clientId) of the `devops-js-app` application copied from the Azure portal.
1. Find the key `https://login.microsoftonline.com/Enter_the_Tenant_Info_Here` and replace the existing value with `https://login.microsoftonline.com/<your-tenant-id>`.
1. Find the key `Enter_the_Redirect_Uri_Here` and replace the existing value with the base address of the `devops-js-app` project (by default `http://localhost:8081`).

## Running the sample

```console
    npm start
```

## Explore the sample

1. Open your browser and navigate to `http://localhost:8081`.
1. Click the **sign-in** button on the top right corner.
1. Once signed-in, select the **Call DevOps Rest API** button at the center.