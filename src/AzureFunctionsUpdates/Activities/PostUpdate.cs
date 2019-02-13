using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities
{
    public static class PostUpdate
    {
        [FunctionName(nameof(PostUpdate))]
        public static async Task Run(
            [ActivityTrigger] RepositoryRelease newRelease,
            ILogger logger)
        {

        }
    }
}
