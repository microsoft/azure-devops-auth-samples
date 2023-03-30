using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ServicePrincipalsSamples.Aad;
using ServicePrincipalsSamples.AdoClient;
using ServicePrincipalsSamples.Settings;

namespace ServicePrincipalsSamples
{
    static class Program
    {
        static async Task Main()
        {
            bool showMenu = true;
            while (showMenu)
            {
                try
                {
                    AdoAuthenticationMode mode = GetAdoAuthenticationMode();
                    showMenu = false;

                    var config = AppConfiguration.ReadFromJsonFile();
                    config.Ado.AdoAuthenticationMode = mode;

                    using var serviceProvider = ConfigureServices(config);
                    await serviceProvider.GetService<App>().Run();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }

        private static AdoAuthenticationMode GetAdoAuthenticationMode()
        {
            Console.Write("Azure DevOps authentication mode:\n" +
                        "1. Azure AD Service Principal\n" +
                        "2. ADO PAT\n" +
                        "Select (Default: Azure AD Service Principal): ");

            var optionString = Console.ReadLine();

            if (string.IsNullOrEmpty(optionString))
            {
                return AdoAuthenticationMode.AadServicePrincipal;
            }
            else if (int.TryParse(optionString, out int optionNumber) && Enum.IsDefined(typeof(AdoAuthenticationMode), optionNumber))
            {
                return (AdoAuthenticationMode)optionNumber;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported authentication mechanism: {optionString}");
            }
        }

        private static ServiceProvider ConfigureServices(AppConfiguration config)
        {
            var services = new ServiceCollection();

            services
                .AddSingleton(config)
                .AddSingleton<AadClient>()
                .AddSingleton<AdoConnection>()
                .AddSingleton<HttpClient>();

            services
                .AddTransient<App>()
                .AddTransient<AadAccessTokenHandler>();

            // Register Azure DevOps clients
            services
                .AddTransient<MemberEntitlementsClient>()
                .AddTransient<GraphClient>()
                .AddTransient<WorkItemsClient>()
                .AddTransient<ProjectsClient>();

            // Register simple REST client without client libs
            services
                .AddHttpClient<IAdoRestClient, AdoRestClient>(client =>
                {
                    client.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
                    AppConfiguration.AppUserAgent.ForEach(header => client.DefaultRequestHeaders.UserAgent.Add(header));
                })
                .AddHttpMessageHandler<AadAccessTokenHandler>();

            return services.BuildServiceProvider();
        }
    }
}
