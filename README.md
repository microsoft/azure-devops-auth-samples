This sample .NET MVC web app shows how to use the OAuth 2.0 capabilities in Visual Studio Online to prompt users to authorize your app and to acquire an access token and refresh token to access their Visual Studio Online resources.

To learn more about OAuth in Visual Studio Online, see [Authorize access with OAuth 2.0](http://www.visualstudio.com/integrate/get-started/get-started-auth-oauth2-vsi)

###Pre-requisities###

These instructions assume you will be deploying this sample app to an Azure web site. To learn more and to get started, visit [Get started with Azure Web Sites and ASP.NET](http://azure.microsoft.com/en-us/documentation/articles/web-sites-dotnet-get-started).

###Setup###
1. Register an OAuth client app on Visual Studio Online (https://app.vssps.visualstudio.com/app/register) (note: the callback URL should be https://yoursite.azurewebsites.net/oauth/callback, where "yoursite" is the name of your Azure web site)
2. Clone this repository
3. Open the solution (VSOClientOAuthSample.sln) in Visual Studio 2013 (or later)
4. Update the following settings in web.config to match the values in the app you just registered:
  *  App ID
  *  App Secret
  *  Scope (space separated)
  *  Callback URL
5. Build (this will trigger a NuGet package restore, which will pull in all dependencies of the project)
6. Deploy the app.

###Run###
1. Navigate to the app (https://yoursite.azurewebsites.net)
2. Confirm your App ID, scope, and callback URL are showing properly (lower left)
3. Click the Start button
4. Sign in to Visual Studio Online (if necessary)
5. Review and accept the authorization request

If everything is setup properly, Visual Studio Online will generate an access token and refresh token and both values will be displayed. **As an app developer, you would always keep these values secret**. Also a new authorization will appear on [your profile page](https://app.vssps.visualstudio.com/Profile/View).

With this access token, you can call [Visual Studio Online REST APIs](http://www.visualstudio.com/integrate/reference/reference-vso-overview-vsi) by passing the access token in the Authorization header. For example:

```
Authorization: Bearer {your access token}
```

