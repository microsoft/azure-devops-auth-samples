using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountProviderSample
{
    public struct CustomCacheKey
    {
        public CustomCacheKey(string[] scopes, string userIdentifier, string tenantId = null)
        {
            Scopes = scopes;
            UserIdentifier = userIdentifier;
            TenantId = tenantId;
        }

        public string[] Scopes { get; }
        public string UserIdentifier { get; }
        public string TenantId { get; }
    }
}
