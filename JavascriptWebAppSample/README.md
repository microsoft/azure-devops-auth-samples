# Javascript Web App Sample

For javascript web applications that want access to resources like VSTS REST API's, they will have to support an authentication flow for their users. The [Azure Active Directory Authentication Library (ADAL) for JS](https://github.com/AzureAD/azure-activedirectory-library-for-js) enables javascript application developers to setup inerative authentication flows and obtain access tokens for API usage.

## Sample Application

This buildable sample will walk you through the steps to create a single page javascript application which uses ADAL to authenticate a user via an interactive prompt and display all projects contained in a VSTS account/TFS collection.

To run this sample you will need:
* Http-server. You can download [NPM http server](https://www.npmjs.com/package/http-server) if you need one.
* An Azure Active Directory (AAD) tenant. If you do not have one, follow these [steps to set up an AAD](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-howto-tenant)
* A user account in your AAD tenant
* A VSTS account backed by your AAD tenant where your user account has access. If you have an existing VSTS account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your VSTS account](https://www.visualstudio.com/en-us/docs/setup-admin/team-services/manage-organization-access-for-your-account-vs)

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Register the sample application with you Azure Active Directory tenant

1. Sign in to the [Azure Portal](https://portal.azure.com).
2. On the top bar, click on your account and under the Directory list, choose the Active Directory tenant where you wish to register your application.
3. On the left hand navigation menu, select `Azure Active Directory`.
4. Click on `App registrations` and select `New application registration` from the top bar.
5. Enter a `name` for you application, ex. "Adal JS sample", choose `Web app/API` for `application type`, and enter `http://localhost:8080` for the `Redirect URI`. Finally click `create` at the bottom of the screen.
6. Save the `Application ID` from your new application registration. You will need it later in this sample.
7. Grant permissions for VSTS. Click `Required permissions` -> `add` -> `1 Select an API` -> type in and select `Microsoft Visual Studio Team Services` -> check the box for `Delegated Permissions` -> click `Select` -> click `Done` -> click `Grant Permissions` -> click `Yes`.

## Step 3: Run the sample

1. Open `index.html` in [Visual Studio Code](https://code.visualstudio.com/download) or another text editor or IDE.
2. Inside `index.html` there is a section called `Input Vars` you will need to update to run the sample:
    * `clientId` - (Required) update this with the `application id` you saved from step 2.6 above
    * `replyUri` - (optional)  In single page apps this should be the app URL itself. We have set this to `http://localhost:8080`(where we will host our app), but if you are hosting your app at another URL please change this value and the reply URI in `portal.azure.com`
    * `logoutredirectUri` - (optional) update if you are hosting your app at a location other than `http://localhost:8080`
    * `vstsApi` - (Required) update this with your VSTS/TFS collection, e.g. http://myaccount.visualstudio.com/DefaultCollection/_apis/projects?api-version=2.0 for VSTS or http://myserver:8080/tfs/DefaultCollection/_apis/projects?api-version=2.0 for TFS. If you would like to the sample to run a different VSTS API please change the entire string. [Learn more about other VSTS REST API's](https://www.visualstudio.com/en-us/docs/integrate/get-started/rest/basics)
    * `vstsResourceId` - Do not change this value. It is used to receive VSTS ADAL authentication tokens
3. Navigate to the ADAL JS sample in cloned repo `vsts-auth-samples/JavascriptWebAppSample/` and start your http-server which will by default serve `index.html` at `http://localhost:8080`.
4. Navigate to `http://localhost:8080`. Sign in with a user account from your AAD tenant which has access to the VSTS account specified in the `vstsApi`. All projects contained in the account should be displayed.




