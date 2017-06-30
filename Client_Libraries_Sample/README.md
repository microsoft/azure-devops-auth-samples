# Client Libraries Sample

The [Client Libraries](https://www.visualstudio.com/en-us/docs/integrate/get-started/client-libraries/dotnet) are .NET libraries made to simplify integration with Visual Studio Team Services and Team Foundation Server (2015 and later). They allow access to both the Traditional Client Object Model and [new REST APIs](https://www.visualstudio.com/en-us/docs/integrate/api/overview).

## Sample Application

 This buildable sample will walk you through the steps to create an application which uses the [Client Libraries](https://www.visualstudio.com/en-us/docs/integrate/get-started/client-libraries/dotnet) to open an interactive login prompt and use that authentication state to execute a user pre-defined query written in [Work Item Query Language](https://msdn.microsoft.com/en-us/library/bb130198(v=vs.90).aspx). Query results are output into the console.

 ## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git
```

## Step 2: Run the sample

1. Navigate to the sample in cloned repo `vsts-auth-samples/Client_Libraries_Sample/ClientLibrariesConsoleApp/`
2. Use [Nuget package restore](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore) to ensure you have all dependencies installed
3. Open the solution file `ClientLibrariesConsoleApp.sln` in [Visual Studio IDE 2017](https://www.visualstudio.com/downloads/)
4. Open CS file `Program.cs` and there will be 2 input fields:
    * `collectionUri` - Mutable value. This is the url to your VSTS/TFS account.
    * `query` - Mutable value. This is the WIQL query you want to execute. Replace this with any query you want to execute.
5. Build and run solution. After running you should see a list of the IDs all work items which match your query restrictions.