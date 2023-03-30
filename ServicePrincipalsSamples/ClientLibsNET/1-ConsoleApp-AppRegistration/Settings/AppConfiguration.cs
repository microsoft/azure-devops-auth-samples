using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace ServicePrincipalsSamples.Settings
{
    public class AppConfiguration
    {
        /// <summary>
        /// Custom UserAgent header - "Identity.ServicePrincipalsSamples/1.0 (1-ConsoleApp-AppRegistration)"
        /// </summary>
        public static List<ProductInfoHeaderValue> AppUserAgent { get; } = new()
        {
            new ProductInfoHeaderValue("Identity.ServicePrincipalsSamples", "1.0"),
            new ProductInfoHeaderValue("(1-ConsoleApp-AppRegistration)")
        };

        public AadConfiguration Aad { get; set; }

        public AdoConfiguration Ado { get; set; }

        /// <summary>
        /// Reads the configuration from a json file
        /// </summary>
        /// <returns>Configuration read from the json file</returns>
        public static AppConfiguration ReadFromJsonFile()
        {
            var path = "Settings/appsettings.json";
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environmentName != null)
            {
                path = $"Settings/appsettings.{environmentName}.json";
            }

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path, optional: false, reloadOnChange: true)
                .Build().Get<AppConfiguration>();
        }
    }
}

