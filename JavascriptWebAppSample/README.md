# Javascript Web App Sample

For javascript web applications that want access to resources like Azure DevOps REST API's, they will have to support an authentication flow for their users. The [Azure Active Directory Authentication Library (ADAL) for JS](https://github.com/AzureAD/azure-activedirectory-library-for-js) enables javascript application developers to setup inerative authentication flows and obtain access tokens for API usage.

## Sample Application

This buildable sample will walk you through the steps to create a single page javascript application which uses ADAL to authenticate a user via an interactive prompt and display all projects contained in a Azure DevOps account/TFS collection.

To run this sample you will need:
* Http-server. You can download [NPM http server](https://www.npmjs.com/package/http-server) if you need one.
* An Azure Active Directory (AAD) tenant. If you do not have one, follow these [steps to set up an AAD](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-howto-tenant)
* A user account in your AAD tenant
* A Azure DevOps account backed by your AAD tenant where your user account has access. If you have an existing Azure DevOps account not connected to your AAD tenant follow these [steps to connect your AAD tenant to your Azure DevOps account](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/manage-azure-active-directory-groups-vsts?view=vsts&tabs=new-nav)

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
5. Enter a `name` for you application, ex. "Adal JS sample", choose `Web app/API` for `application type`, and enter `http://localhost:8081` for the `Sign-on URL`. Finally click `create` at the bottom of the screen.
6. Save the `Application ID` from your new application registration. You will need it later in this sample.
7. Grant permissions for Azure DevOps. Click `Settings` -> `Required permissions` -> `add` -> `1 Select an API` -> type in and select `Azure DevOps (Microsoft Visual Studio Team Services)` -> check the box for `Delegated Permissions` -> click `Select` -> click `Done` -> click `Grant Permissions` -> click `Yes`.
8. Click on `Manifest` -> set the value for `oauth2AllowImplicitFlow` to `true` -> click `Save`.

## Step 3: Run the sample

1. Open `index.html` in [Visual Studio Code](https://code.visualstudio.com/download) or another text editor or IDE.
2. Inside `index.html` there is a section called `Input Vars` you will need to update to run the sample:
    * `clientId` - (Required) update this with the `application id` you saved from step 2.6 above
    * `replyUri` - (optional)  In single page apps this should be the app URL itself. We have set this to `http://localhost:8081`(where we will host our app), but if you are hosting your app at another URL please change this value and the reply URI in `portal.azure.com`
    * `logoutredirectUri` - (optional) update if you are hosting your app at a location other than `http://localhost:8081`
    * `azureDevOpsApi` - (Required) update this with your Azure DevOps/TFS collection, e.g. http://dev.azure.com/organization/_apis/projects?api-version=4.0 for Azure DevOps or http://myserver:8080/tfs/DefaultCollection/_apis/projects?api-version=4.0 for TFS. If you would like the sample to run a different Azure DevOps API please change the entire string. [Learn more about other Azure DevOps REST API's](https://docs.microsoft.com/en-us/rest/api/vsts/?view=vsts-rest-4.1&viewFallbackFrom=vsts)
    * `azureDevOpsResourceId` - Do not change this value. It is used to receive Azure DevOps ADAL authentication tokens
3. Navigate to the ADAL JS sample in cloned repo `vsts-auth-samples/JavascriptWebAppSample/` and start your http-server and set it to serve pages at `localhost:8081` which will by default serve `index.html` at `http://localhost:8081`.
4. Navigate to `http://localhost:8081`. Sign in with a user account from your AAD tenant which has access to the VSTS account specified in the `azureDevOpsApi`. All projects contained in the account should be displayed.




