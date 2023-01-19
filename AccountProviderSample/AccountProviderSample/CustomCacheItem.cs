using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using System;

namespace AccountProviderSample
{
    public class CustomCacheItem : IAccountCacheItem
    {
        public CustomCacheItem(AuthenticationResult result)
        {
            UniqueId = result.UniqueId;
            TenantId = result.TenantId;
            Username = result.Account.Username;
            InnerResult = result;
            // ...
        }

        public string UniqueId { get; set; }

        public string TenantId { get; set; }

        public string Username { get; set; }

        public string Environment { get; set; }

        public string IdToken { get; set; }

        public DateTimeOffset ExpiresOn { get; set; }

        public string AccessToken { get; set; }

        public AuthenticationResult InnerResult { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }
    }
}
