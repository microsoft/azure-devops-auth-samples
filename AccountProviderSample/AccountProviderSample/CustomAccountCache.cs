using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountProviderSample
{
    public class CustomAccountCache : IAccountCache
    {
        // This is a simple in-memory cache. In a real scenario, you would want to use a persistent cache.
        private readonly ConcurrentDictionary<CustomCacheKey, CustomCacheItem> inMemoryCache = new ConcurrentDictionary<CustomCacheKey, CustomCacheItem>();

        private string ClientId { get; }
        public string Authority { get; }

        public CustomAccountCache(string clientId, string authority)
        {
            ClientId = clientId;
            Authority = authority;
        }

        private CustomCacheKey ConstructKey(string[] scopes, string userIdentifier, string tenantId = null)
        {
            return new CustomCacheKey(scopes, userIdentifier, tenantId);
        }

        public async Task<IAccountCacheItem> AcquireTokenInteractiveAsync(string[] scopes, Prompt prompt = default, string userIdentifier = null, string tenantId = null)
        {
            // This is a sub-optimal implementation, since the PublicApplicationClient could be reused.
            var builder = PublicClientApplicationBuilder.Create(ClientId).WithAuthority(Authority);

            var application = builder.Build();
            var query = application.AcquireTokenInteractive(scopes);

            if (!string.IsNullOrWhiteSpace(userIdentifier))
            {
                var account = await application.GetAccountAsync(userIdentifier);
                query.WithAccount(account);
            }

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                query.WithTenantId(tenantId);
            }

            var result = await query.ExecuteAsync();

            if (result != null)
            {
                var key = ConstructKey(scopes, userIdentifier, tenantId);
                var value = new CustomCacheItem(result);
                inMemoryCache.AddOrUpdate(key, value, (k, v) => value);
                return value;
            }

            return null;
        }

        public async Task<IAccountCacheItem> AcquireTokenSilentAsync(string[] scopes, string userIdentifier, string tenantId = null)
        {
            var key = ConstructKey(scopes, userIdentifier, tenantId);
            if (inMemoryCache.TryGetValue(key, out var result))
            {
                return result;
            }

            return await AcquireTokenInteractiveAsync(scopes, Prompt.NoPrompt, userIdentifier, tenantId);
        }

        public Task DeleteItemAsync(IAccountCacheItem token)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAnyUserIdentifierAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccountCacheItem> GetItems()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAccountCacheItem>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccountCacheItem> GetVsoEndpointToken(IAccountCacheItem tokenCacheItem)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAccountCacheItem>> GetVsoEndpointTokenAsync(IAccountCacheItem tokenCacheItem)
        {
            throw new NotImplementedException();
        }
    }
}
