# PowerShell Sample

For PowerShell applications which can support interative login prompts, the [ADAL.PS](https://www.powershellgallery.com/packages/ADAL.PS/5.2.6.1) module makes it easy to setup authentication flows for users.

## Prerequisites

To run this sample for an [Azure Active Directory](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-whatis) backed Azure DevOps account you will need:
* [PowerShell](https://docs.microsoft.com/en-us/powershell/scripting/overview?view=powershell-7)
* An Azure Active Directory (AAD) tenant. If you do not have one, follow these [steps to set up an AAD tenant.](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-create-new-tenant)
* A user account in your AAD tenant
* A Azure DevOps account backed by your AAD tenant where your user account has access. If you have an existing Azure DevOps account, but it's not connected to your AAD tenant follow these [steps to connect your AAD tenant to your Azure DevOps account.](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/manage-azure-active-directory-groups-vsts?view=vsts&tabs=new-nav)

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Register the AAD App with you tenant

1. Sign in to the [Azure Portal](https://portal.azure.com).
1. On the top bar, click on your account and under the Directory list, choose the Active Directory tenant where you wish to register your application.
1. On the left hand navigation menu, select `Azure Active Directory`.
1. Click on `App registrations` and select `New registration` from the top bar.
1. Enter a `Name` for you application, e.g. "Adal PowerShell Native App".
1. Supported account types: `Accounts in this organizational directory only`
1. Redirect URI: `Public client/native (mobile & desktop)` for application type, and enter `urn:ietf:wg:oauth:2.0:oob` in the text box. 
1. Finally click `Register` at the bottom of the screen.
1. Save the `Application (client) ID` and `Directory (tenant) ID` from your new application registration. You will need it later in this sample.
1. Grant permissions for Azure DevOps. Click `API permissions` -> `Add a permission` -> Select `Azure DevOps` -> Click on `Delegated Permissions` -> Click `user_impersonation` -> Click `Add permissions`.

## Step 3: Run the sample (AAD backed Azure DevOps account)

1. Navigate to `PowerShellSample.ps1`.
1. There is a config section at the top where you will update your values:
    * `$azureDevOpsOrgUrl` - update this with the url to your Azure DevOps collection, e.g. http://dev.azure.com/fabrikam.
    * `$aadClientId` - update this with the `Application (client) ID` you saved earlier.
    * `$tenantId` - update this with the `Directory (tenant) ID` you saved earlier.