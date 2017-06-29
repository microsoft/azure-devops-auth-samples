# Device Profile Sample

For a headless text output client application, it is not possible authenticate through an interactive prompt. Instead a text only approach is necessary. This flow leverages a user's external device (i.e. phone) to authenticate through an interactive login prompt and pass the auth token to the headless application. For more information [click here](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/?v=17.23h).

## Sample Application

This buildable sample will walk you through the steps to create a client-side console application which uses ADAL to authenticate a user via the [Device Profile flow](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-deviceprofile/?v=17.23h) and returns a JSON string containing all VSTS identity data for the authenticated user.

## Step 1: Clone or download vsts-auth-samples repository

From a shell or command line: 
```no-highlight
git clone https://github.com/Microsoft/vsts-auth-samples.git