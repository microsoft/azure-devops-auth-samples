using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json.Nodes;
using ServicePrincipalsSamples.Settings;

namespace ServicePrincipalsSamples.AdoClient
{
    public interface IAdoRestClient
    {
        Task<JsonNode> GetWorkItem(int workItemId);

        Task<JsonNode> AddAadUserToGroup(string adoGroupSD, string userObjectId);
    }

    /// <summary>
    /// Azure DevOps simple REST client not using client libs
    /// </summary>
    public class AdoRestClient : IAdoRestClient
    {
        internal const string SpsUrlFragment = "vssps";

        private readonly AppConfiguration config;
        private readonly HttpClient httpClient;

        public AdoRestClient(AppConfiguration config, HttpClient httpClient)
        {
            this.config = config;
            this.httpClient = httpClient;
        }

        public async Task<JsonNode> GetWorkItem(int workItemId)
        {
            var path = $"_apis/wit/workItems/{workItemId}";
            var url = CreateBaseAdoOrgUrl(path, version: "7.1-preview.3");

            var responseString = await httpClient.GetStringAsync(url);

            return JsonNode.Parse(responseString);
        }

        public async Task<JsonNode> AddAadUserToGroup(string adoGroupSD, string userObjectId)
        {
            var path = "_apis/Graph/users";
            var queryParams = new Dictionary<string, string>
            {
                { "groupDescriptors", adoGroupSD }
            };
            var url = CreateAdoOrgUrlForService(SpsUrlFragment, path, queryParams);

            var user = new
            {
                originId = userObjectId
            };

            var response = await httpClient.PostAsJsonAsync(url, user);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonNode.Parse(responseString);
        }

        #region Private methods

        private string CreateBaseAdoOrgUrl(string path, Dictionary<string, string> queryParams = null, string version = null)
        {
            return CreateAdoOrgUrl(config.Ado.OrganizationUrl, path, queryParams, version);
        }

        private string CreateAdoOrgUrlForService(string serviceUrlFragment, string path, Dictionary<string, string> queryParams = null, string version = null)
        {
            return CreateAdoOrgUrl(GetOrgUrlWithFragment(serviceUrlFragment), path, queryParams, version);
        }

        private string CreateAdoOrgUrl(Uri baseUrl, string path, Dictionary<string, string> queryParams, string version = null)
        {
            var uriBuilder = new UriBuilder(baseUrl)
            {
                Path = $"{config.Ado.Organization}/{path}"
            };

            queryParams ??= new Dictionary<string, string>();
            queryParams.Add("api-version", version ?? AdoConfiguration.DefaultApiVersion);

            return QueryHelpers.AddQueryString(uriBuilder.ToString(), queryParams);
        }

        /// <summary>
        /// Returns the URL with the service (if specified). Example: https://vssps.dev.azure.com/{AdoOrgName}
        /// </summary>
        /// <param name="serviceUrlFragment">If not provided, it default to the ADO base URL</param>
        public Uri GetOrgUrlWithFragment(string serviceUrlFragment)
        {
            var baseOrgUrl = config.Ado.OrganizationUrl;
            var uriBuilder = new UriBuilder(baseOrgUrl);
            uriBuilder.Host = $"{serviceUrlFragment}.{uriBuilder.Host}";
            return uriBuilder.Uri;
        }

        #endregion
    }
}
