# AspNetCore OAuth implementation for Azure DevOps 

This is a sample implementation of how to integrate Azure DevOps OAuth into 
your ASP.NET Core application. 

## Using in your application

The bulk of the OAuth code is contained inside
the [AzureDevOpsOAuthTools.cs](AzureDevOpsOAuthTools.cs) file. That file should
be portable to your web application. After including that you will need
to do the following in `Startup.cs`

```cs
.AddAzureDevOps(options =>
{
    // These represent the App ID and Client Secret portions of your Azure DevOps Application
    // settings
    options.ClientId = Configuration["AzureAppId"];
    options.ClientSecret = Configuration["AzureClientSecret"];

    // These must be the *exact* scopes defined in your application
    options.Scope.Add("vso.build");
    options.Scope.Add("vso.identity");
    options.Scope.Add("vso.work");
});
```

## Running this sample
In order to run this sample you will need to do the following:

[Register](https://app.vsaex.visualstudio.com/app/register) an application to 
use in the sample. Recommend that you use the following for local development

- Application Website: "https://localhost:44351"
- Authorization Callback URL: "https://localhost:44351/signin-azdo"

The scopes should be set to vso.build, vso.identity and vso.work in order for
the application to work with the sample. If you choose different scopes you 
will need to adjust the `options.Scopes.Add` lines in `Startup.cs`.

Setup the user secrets expected by the application:

``` cmd
> dotnet user-secrets add AzureAppId <the app id of your application>
> dotnet user-secrets add AzureClientSecret <the client secret of your application>
```

**Note**: Make sure you use the Client Secret, not the App Secret for the 
second entry there.


