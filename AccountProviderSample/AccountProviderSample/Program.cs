using Microsoft.VisualStudio.Services.Client.AccountManagement;
using System;
using System.Diagnostics;
using System.Globalization;

namespace AccountProviderSample
{
    internal class Program
    {
        private static string AadInstance = "https://login.microsoftonline.com/{0}"; // Change for other AAD instances
        private static string TenantId = "AAD_Tenant_ID";
        private static string UserId = "User_UPN";
        private static string ClientId = "AAD_Application_ID";
        private static string Authority = String.Format(CultureInfo.InvariantCulture, AadInstance, TenantId);

        static void Main(string[] args)
        {
            var accountProvider = new VSAccountProvider(null);
            accountProvider.SetAccountCache(new CustomAccountCache(ClientId, Authority));

            // This is the scope for the Azure DevOps Resource. Feel free to replace with your desired scopes.
            var scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" };

            // This is the handle of the window that will be used to display the authentication dialog.
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
            var accountKey = new AccountKey(UserId, Guid.Parse(TenantId));
            var result = accountProvider.AcquireTokenAsync(scopes, TenantId, UserId, handle, accountKey).GetAwaiter().GetResult();

            Console.WriteLine(result.AccessToken);
        }
    }
}
