using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Core.WebApi;

namespace ServicePrincipalsSamples.AdoClient
{
    public class ProjectsClient : AdoClientBase<ProjectHttpClient>
    {
        public ProjectsClient(AdoConnection adoConnection) : base(adoConnection) { }

        public async Task<IEnumerable<TeamProjectReference>> ListProjects()
        {
            var projects = await GetClient().GetProjects();

            return projects;
        }

        public async Task CreateProject(string projectName)
        {
            var versionControlDictionary = new Dictionary<string, string>
            {
                { "sourceControlType", "Git" }
            };

            var processTemplateDictionary = new Dictionary<string, string>
            {
                { "templateTypeId", "6b724908-ef14-45cf-84f8-768b5384da45" }
            };

            var teamProject = new TeamProject
            {
                Name = projectName,
                Description = "This project was created from the sample console application.",
                Capabilities = new Dictionary<string, Dictionary<string, string>>
                {
                    { "versioncontrol", versionControlDictionary },
                    { "processTemplate", processTemplateDictionary }
                }
            };

            await GetClient().QueueCreateProject(teamProject);

            Console.WriteLine($"Project '{projectName}' queued for creation...");
        }
    }
}
