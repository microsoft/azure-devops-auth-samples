
/**
 * Configuration object to be passed to MSAL instance on creation. 
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md 
 */

const msalConfig = {
    auth: {
        clientId: "Enter_the_Application_Id_Here", // This is the ONLY mandatory field that you need to supply.
        authority: "https://login.microsoftonline.com/Enter_the_Tenant_Id_Here", // Defaults to "https://login.microsoftonline.com/common"
        redirectUri: "http://localhost:8081", // You must register this URI on Azure Portal/App Registration. Defaults to window.location.href
        postLogoutRedirectUri: "http://localhost:8081", // Simply remove this line if you would like navigate to index page after logout.
        navigateToLoginRequestUrl: false, // If "true", will navigate back to the original request location before processing the auth code response.
    },
    cache: {
        cacheLocation: "sessionStorage", // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO btw tabs.
        storeAuthStateInCookie: false, // If you wish to store cache items in cookies as well as browser cache, set this to "true".
    },
    system: {
        loggerOptions: {
            loggerCallback: (level, message, containsPii) => {
                if (containsPii) {
                    return;
                }
                switch (level) {
                    case msal.LogLevel.Error:
                        console.error(message);
                        return;
                    case msal.LogLevel.Info:
                        console.info(message);
                        return;
                    case msal.LogLevel.Verbose:
                        console.debug(message);
                        return;
                    case msal.LogLevel.Warning:
                        console.warn(message);
                        return;
                }
            }
        }
    }
};

// Add here the endpoints for services you would like to use.
const apiConfig = {
    endpoint: "https://dev.azure.com/Enter_the_Tenant_Name_Here/_apis/projects?api-version=4.0", // e.g. https://dev.azure.com/fabrikam/_apis/projects?api-version=4.0
    scopes: ["499b84ac-1321-427f-aa17-267ca6975798/.default"] // 499b84ac-1321-427f-aa17-267ca6975798 is the App Id of Azure DevOps Service
};

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit: 
 * https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
const loginRequest = {
    scopes: ["openid", "profile"],
};

/**
 * Add here the scopes to request when obtaining an access token for a web API. For more information, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
const tokenRequest = {
    scopes: apiConfig.scopes,
    forceRefresh: false // Set this to "true" to skip a cached token and go to the server to get a new token
};

/**
 * An optional silentRequest object can be used to achieve silent SSO
 * between applications by providing a "login_hint" property.
 */

// const silentRequest = {
//   scopes: ["openid", "profile"],
//   loginHint: "example@domain.net"
// };
