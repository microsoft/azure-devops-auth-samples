using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.MemberEntitlementManagement.WebApi;
using ServicePrincipalsSamples.AdoClient;

namespace ServicePrincipalsSamples
{
    /// <summary>
    /// This sample shows how to perform some actions in Azure DevOps using an Application Service Principal.
    /// </summary>
    public class App
    {
        private readonly MemberEntitlementsClient memberEntitlementsClient;
        private readonly GraphClient graphClient;
        private readonly WorkItemsClient workItemsClient;
        private readonly ProjectsClient projectsClient;        

        private readonly IAdoRestClient adoRestClient;

        public App(IServiceProvider serviceProvider)
        {
            memberEntitlementsClient = serviceProvider.GetRequiredService<MemberEntitlementsClient>();
            graphClient = serviceProvider.GetService<GraphClient>();
            workItemsClient = serviceProvider.GetService<WorkItemsClient>();
            projectsClient = serviceProvider.GetService<ProjectsClient>();

            adoRestClient = serviceProvider.GetService<IAdoRestClient>();
        }

        public async Task Run()
        {
            var appMenu = CreateMenu();

            bool showMenu = true;
            while (showMenu)
            {
                try
                {
                    showMenu = await appMenu.DisplayMenu();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }

        private AppMenu CreateMenu()
        {
            var appMenu = new AppMenu();

            appMenu
                .AddOptionGroup("Member entitlements management")
                .AddOption("List users in organization", SearchUserEntitlements)
                .AddOption("Get Service Principal entitlement (me)", GetServicePrincipalEntitlementMe);

            appMenu
                .AddOptionGroup("Graph")
                .AddOption("List Azure AD users", ListAadUsers)
                .AddOption("Add user to a group", AddAadUserToGroup)
                .AddOption("Add user to a group (REST Client)", AddAadUserToGroupWithRestClient);

            appMenu
                .AddOptionGroup("Work items")
                .AddOption("Create work item", CreateWorkItem)
                .AddOption("Get work item", GetWorkItem)
                .AddOption("Get work item (REST Client)", GetWorkItemWithRestClient);

            appMenu
                .AddOptionGroup("Projects")
                .AddOption("List Projects", ListProjects)
                .AddOption("Create project", CreateProject);

            appMenu
                .AddOptionGroup("Other")
                .AddOption("Get Authorized identity", GetAuthorizedIdentity);

            return appMenu;
        }

        private async Task SearchUserEntitlements()
        {
            var users = await memberEntitlementsClient.SearchUserEntitlements();

            PrintUserEntitlements(users);
        }

        private static void PrintUserEntitlements(IList<UserEntitlement> userEntitlements)
        {
            var tableResult = new TableResult<UserEntitlement>()
                .AddColumn(-60, "Subject Descriptor", (UserEntitlement e) => e.User.Descriptor)
                .AddColumn(-30, "Display Name", (UserEntitlement e) => e.User.DisplayName)
                .AddColumn(-40, "Principal Name", (UserEntitlement e) => e.User.PrincipalName)
                .AddColumn(-20, "License", (UserEntitlement e) => e.AccessLevel.LicenseDisplayName);

            tableResult.Display(userEntitlements);
        }


        private async Task GetServicePrincipalEntitlementMe()
        {
            var entitlement = await memberEntitlementsClient.GetServicePrincipalEntitlementMe();

            PrintServicePrincipals(new List<ServicePrincipalEntitlement>() { entitlement });
        }

        private static void PrintServicePrincipals(IList<ServicePrincipalEntitlement> servicePrincipalEntitlements)
        {
            var tableResult = new TableResult<ServicePrincipalEntitlement>()
                .AddColumn(-60, "Subject Descriptor", (ServicePrincipalEntitlement e) => e.ServicePrincipal.Descriptor)
                .AddColumn(-30, "Display Name", (ServicePrincipalEntitlement e) => e.ServicePrincipal.DisplayName)
                .AddColumn(-40, "Application ID", (ServicePrincipalEntitlement e) => e.ServicePrincipal.ApplicationId)
                .AddColumn(-40, "Object ID", (ServicePrincipalEntitlement e) => e.ServicePrincipal.PrincipalName)
                .AddColumn(-20, "License", (ServicePrincipalEntitlement e) => e.AccessLevel.LicenseDisplayName);

            tableResult.Display(servicePrincipalEntitlements);
        }

        private async Task CreateWorkItem()
        {
            Console.Write("Project name: ");
            string projectName = Console.ReadLine();
            Console.Write("Work item title: ");
            string workItemTitle = Console.ReadLine();

            var workItem = await workItemsClient.CreateWorkItem(projectName, workItemTitle);
            PrintWorkItem(workItem);
        }

        private async Task GetWorkItemWithRestClient()
        {
            await GetWorkItem(useClientLib: false);
        }

        private async Task GetWorkItem()
        {
            await GetWorkItem(useClientLib: true);
        }

        private async Task GetWorkItem(bool useClientLib)
        {
            Console.Write("Work item ID: ");
            int workItemId = Convert.ToInt32(Console.ReadLine());

            if (useClientLib)
            {
                var workItem = await workItemsClient.GetWorkItem(workItemId);
                PrintWorkItem(workItem);
            }
            else
            {
                var workItem = await adoRestClient.GetWorkItem(workItemId);
                Console.WriteLine($"Raw work item:\n {workItem}");
            }
        }

        private static void PrintWorkItem(WorkItem workItem)
        {
            workItem.Fields.TryGetValue("System.Title", out var title);
            workItem.Fields.TryGetValue("System.WorkItemType", out var type);

            Console.WriteLine($"Work item - {type} {workItem.Id}: {title}");

            foreach (var field in workItem.Fields)
            {
                Console.WriteLine("- {0}: {1}", field.Key, field.Value);
            }
        }

        private async Task CreateProject()
        {
            Console.Write("Project name: ");
            string projectName = Console.ReadLine();

            await projectsClient.CreateProject(projectName);
        }

        private async Task ListProjects()
        {
            var projects = await projectsClient.ListProjects();

            Console.WriteLine("Projects:");
            foreach (var project in projects)
            {
                Console.WriteLine($"- {project.Name} (ID: {project.Id})");
            }
        }

        private async Task AddAadUserToGroupWithRestClient()
        {
            await AddAadUserToGroup(useClientLib: false);
        }

        private async Task AddAadUserToGroup()
        {
            await AddAadUserToGroup(useClientLib: true);
        }

        private async Task AddAadUserToGroup(bool useClientLib)
        {
            Console.Write("Group subject descriptor: ");
            string groupSD = Console.ReadLine();
            Console.Write("User Object ID: ");
            string userObjectId = Console.ReadLine();

            if (useClientLib)
            {
                var user = await graphClient.AddAadUserToGroup(groupSD, userObjectId);
                Console.WriteLine($"User '{user.DisplayName}' added successfully to the group.");
            }
            else
            {
                await adoRestClient.AddAadUserToGroup(groupSD, userObjectId);
                Console.WriteLine("User added successfully to the group.");
            }
        }

        private async Task ListAadUsers()
        {
            var users = await graphClient.ListAadUsers();

            PrintGraphUsers(users.ToList());
        }

        private static void PrintGraphUsers(IList<GraphUser> users)
        {
            var tableResult = new TableResult<GraphUser>()
                .AddColumn(-60, "Subject Descriptor", (e) => e.Descriptor)
                .AddColumn(-30, "Display Name", (e) => e.DisplayName)
                .AddColumn(-40, "Principal Name", (e) => e.PrincipalName);

            tableResult.Display(users);
        }

        private Task GetAuthorizedIdentity()
        {
            var identity = graphClient.GetAuthorizedIdentity();
            Console.WriteLine("Authorized Identity\n" +
                $"- ID: {identity.Id}\n" +
                $"- Identity Type: {identity.Descriptor.IdentityType}\n" +
                $"- MetaType: {identity.MetaType} \n" +
                $"- Display Name: {identity.DisplayName} \n" +
                $"- Subject Descriptor: {identity.SubjectDescriptor} \n");

            return Task.CompletedTask;
        }
    }
}
