using Microsoft.Identity.Web;
using System;

namespace ServicePrincipalsSamples.Settings
{
    /// <summary>
    /// Settings related to Azure Active Directory (Azure AD)
    /// </summary>
    public class AadConfiguration
    {
        /// <summary>
        /// instance of Azure AD, for example public Azure or a Sovereign cloud (Azure China, Germany, US government, etc ...)
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// The Tenant is:
        /// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// or a domain name associated with the tenant
        /// - or 'organizations' (for a multi-tenant application)
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Guid used by the application to uniquely identify itself to Azure AD
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// URL of the authority.
        /// </summary>
        public Uri Authority
        {
            get
            {
                return new Uri(new Uri(Instance), Tenant);
            }
        }

        /// <summary>
        /// Client secret (application password)
        /// </summary>
        /// <remarks>Daemon applications can authenticate with Azure AD through two mechanisms: ClientSecret
        /// (which is a kind of application password: this property)
        /// or a certificate previously shared with AzureAD during the application registration 
        /// (and identified by the Certificate property below)
        /// </remarks>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The description of the certificate to be used to authenticate your application.
        /// </summary>
        public CertificateDescription Certificate { get; set; }
    }
}
