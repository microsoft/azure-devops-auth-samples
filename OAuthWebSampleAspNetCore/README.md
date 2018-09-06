# VSTS OAuth Web Sample (ASP.NET Core)

This sample web app demonstrates how to authorize and acquire access tokens for VSTS users so your service can interact with VSTS on their behalf.

To learn more about OAuth in Visual Studio Team Services, see [Authorize access to VSTS with OAuth 2.0](https://docs.microsoft.com/vsts/integrate/get-started/authentication/oauth?view=vsts)

## How to configure

1. Register an OAuth client app in Visual Studio Team Services (https://app.vsaex.visualstudio.com/app/register) 

2. Open this project in Visual Studio Code (or your favorite code editor)

3. Update `appsettings.json` with your new client app's ID, scopes (space separated), and callback URL.

4. Store your client secret securly (for development):
    ```
    dotnet user-secrets set "oauth:clientApp:secret" "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI...."
    ```
