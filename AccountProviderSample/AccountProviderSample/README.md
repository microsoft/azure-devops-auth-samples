# Account Provider Sample

For native applications the best way to authenticate and access Azure DevOps resources is using the [Client Libraries](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=vsts). They are .NET libraries made to simplify integration with Azure DevOps and Team Foundation Server (2015 and later). They allow access to both the Traditional Client Object Model and [new REST APIs](https://docs.microsoft.com/en-us/rest/api/vsts/?view=vsts-rest-4.1).

## Sample Application

 The Account Provider sample shows a simplified code flow for handling the account cache by the consumer. In combination with the ability to pass a UI handle inside the code, it is useful in desktop scenarios, e.g.: when working with Windows Forms apps.

 ## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Run the sample

1. Navigate to the sample in cloned repo `vsts-auth-samples/AccountProviderSample/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
3. Open the solution file `AccountProviderSample.sln` in [Visual Studio](https://www.visualstudio.com/downloads/)
4. Open CS file `Program.cs` and change the input values at the top of the class:
    * `AadInstance` - the instance of Azure, for example public Azure or Azure China.
    * `TenantId` - the name or Id of the Azure AD tenant in which this application is registered.
    * `UserId` - user's UPN, usually an email address.
    * `ClientId` - used by the application to uniquely identify itself to Azure AD.
5. Additionally, you may want to change also the following variables:
    * `scopes` - the parts of target system you would like to get access to.
    * `handle` - the pointer to your main Window
6. Build and run solution.
