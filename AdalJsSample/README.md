# ADAL JS Sample

The [Azure Active Directory Authentication Library (ADAL)](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-libraries) enables client application developers to authenticate cloud users and obtain access tokens for API usage.

## Sample Application

This buildable sample will walk you through the steps to create single page javascript application which uses ADAL to authenticate a user via an interactive prompt and all VSTS known information associated with the signed in identity.

To run this sample you will need:
* Http-server. You can download [NPM http server](https://www.npmjs.com/package/http-server) if you need one.
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
5. Enter a `name` for you application, ex. "Adal JS sample", choose `Web app/API` for `application type`, and enter `http://localhost:8080` for the `Redirect URI`. Finally click `create` at the bottom of the screen.
6. Save the `Application ID` from your new application registration. You will need it later in this sample.
7. Grant Permissions for VSTS. Click `Required permissions` -> `add` -> `1 Select an API` -> type in and select `Microsoft Visual Studio Team Services` -> check the box for `Have full access to...` -> click `Save` -> click `Grant Permissions` -> click `Yes`.

## Step 3: Run the sample

1. Open `index.html` in [Visual Studio Code](https://code.visualstudio.com/download?wt.mc_id=adw-brandcore-editor-slink-downloads&gclid=EAIaIQobChMItJndsOXH1QIVFJR-Ch3uTgMREAAYASABEgLb2vD_BwE) or another text editor or IDE.
2. Inside `index.html` there is a section of `Input Vars` you can update to run the sample:
    * `clientId` - (Required) update this with the `application id` you saved from `portal.azure.com`
    * `replyUri` - (optional) update this if you are hosting `index.html` at an address other than `hottp://localhost:8080`
    * `vstsApi` - (optional) update this if you would like to the sample to run a different VSTS API
    * `vstsResourceId` - Do not change this value. It is used to receive VSTS ADAL authentication tokens
3. Navigate to the ADAL JS sample in cloned repo `vsts-auth-samples/AdalJsSample/` and start your http-server which will serve `index.hmtl` at `http://localhost:8080`.
4. Navigate to `http://localhost:8080`. Sign in with a user account from your AAD tenant which has access to at least 1 VSTS account. Displayed should be all VSTS known identity infromation about the signed in user account.




