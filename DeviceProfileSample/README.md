# Device Profile Sample

For a headless text output client application, it is not possible authenticate through an interactive prompt. Instead a text only approach is necessary. This flow leverages a user's external device (i.e. phone) to authenticate through an interactive login prompt and pass the auth token to the headless application. For more information [click here](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/?v=17.23h).

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses ADAL to authenticate a user via the [Device Profile flow](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/?v=17.23h) and returns a JSON string containing all account team project data viewable by the authenticated user.

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Install and configure ADAL (optional)

Package: `Microsoft.Identity.Model.Clients.ActiveDirectory` has already been installed and configured in the sample, but if you are adding to your own project you will need to [install and configure it yourself](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory). 

## Step 3: Run the sample

1. Navigate to the sample in cloned repo `vsts-auth-samples/DeviceProfileSample/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
3. Open the solution file `DeviceProfileSample.csproj` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
4. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsCollectionUrl` - Mutable value. This is the url to your VSTS collection, e.g. http://myaccount.visualstudio.com for VSTS.
5. Build and run solution. You should see a console window with instruction on how to authenticate via the Device Profile flow. After authenticating you should see all team project information viewable by the authenticated identity displayed in the console window.