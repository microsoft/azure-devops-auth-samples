using System;

namespace ServicePrincipalsSamples.Settings
{
    public enum AdoAuthenticationMode
    {
        AadServicePrincipal = 1,
        AdoPat = 2
    }

    /// <summary>
    /// Settings related to Azure DevOps
    /// </summary>
    public class AdoConfiguration
    {
        /// <summary>
        /// Azure DevOps API version
        /// </summary>
        public const string DefaultApiVersion = "7.1-preview.1";

        public AdoAuthenticationMode AdoAuthenticationMode { get; set; }

        /// <summary>
        /// Azure DevOps base url
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Azure DevOps org name (https://dev.azure.com/{Organization})
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Azure DevOps Personal Access Token (PAT)
        /// </summary>
        public string Pat { get; set; }

        /// <summary>
        /// Organization URL
        /// </summary>
        public Uri OrganizationUrl
        {
            get
            {
                return new Uri(new Uri(Instance), Organization);
            }
        }
    }
}
