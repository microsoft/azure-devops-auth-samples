# Client Libraries Sample

For native applications the best way to authenticate and access VSTS resources is using the [Client Libraries](https://www.visualstudio.com/en-us/docs/integrate/get-started/client-libraries/dotnet). They are .NET libraries made to simplify integration with Visual Studio Team Services and Team Foundation Server (2015 and later). They allow access to both the Traditional Client Object Model and [new REST APIs](https://www.visualstudio.com/en-us/docs/integrate/api/overview).

## Sample Application

 This buildable sample will walk you through the steps to create an application which uses the [Client Libraries](https://www.visualstudio.com/en-us/docs/integrate/get-started/client-libraries/dotnet) and a personal access token to execute a user pre-defined query written in [Work Item Query Language](https://msdn.microsoft.com/en-us/library/bb130198(v=vs.90).aspx). Query results are output into the console.

 ## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Run the sample

1. Generate a [PAT token](https://docs.microsoft.com/en-us/vsts/organizations/accounts/use-personal-access-tokens-to-authenticate?view=vsts) and grant it "Work items (read)" permission and make a copy of the generated value
2. Navigate to the sample in cloned repo `vsts-auth-samples/ClientLibraryPatAppSample/`
3. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
4. Open the solution file `ClientLibraryPatAppSample.csproj` in [Visual Studio 2017](https://www.visualstudio.com/downloads/)
5. Open CS file `Program.cs` and there is a section with input values to change at the top of the class:
    * `vstsCollectionUrl` - Mutable value. This is the url to your VSTS/TFS collection, e.g. http://myaccount.visualstudio.com for VSTS or http://myserver:8080/tfs/DefaultCollection for TFS.
    * `pat` - Mutable value. This is the value you generated in step 1.
6. Build and run solution. After running you should see a list of the IDs all work items which match your query restrictions.
