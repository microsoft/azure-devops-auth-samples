using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Threading.Tasks;
using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace ServicePrincipalsSamples.AdoClient
{
    public class WorkItemsClient : AdoClientBase<WorkItemTrackingHttpClient>
    {
        public WorkItemsClient(AdoConnection adoConnection) : base(adoConnection) { }

        public async Task<WorkItem> GetWorkItem(int workItemId)
        {
            return await GetClient().GetWorkItemAsync(workItemId);
        }

        public async Task<WorkItem> CreateWorkItem(string projectName, string workItemTitle)
        {
            var patchDocument = new JsonPatchDocument
            {
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = workItemTitle
                }
            };

            return await GetClient().CreateWorkItemAsync(patchDocument, projectName, "Task");
        }
    }
}
