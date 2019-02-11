using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Orchestrations
{
    public static class RepoUpdateOrchestration
    {
        [FunctionName(nameof(RepoUpdateOrchestration))]
        public static async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContextBase context,
            ILogger logger)
        {
            // Read repo links from storage table
            var repositories = await context.CallActivityAsync<IReadOnlyCollection<RepoConfiguration>>(
                nameof(GetRepositories),
                null);

            if (repositories.Any())
            {
                var getReleaseTasks = new List<Task<RepoRelease>>();

                // Fan out over the repos
                foreach (var repo in repositories)
                {
                    getReleaseTasks.Add(context.CallActivityWithRetryAsync<RepoRelease>(
                        nameof(GetRelease),
                        GetDefaultRetryOptions(),
                        repo));
                }

                var releaseResults = await Task.WhenAll(getReleaseTasks);

                // TODO determine if retrieved release info is different that release info from storage

                var postUpdateTasks = new List<Task>();
                // TODO if release is different, fan out for post updates
            }
        }

        private static RetryOptions GetDefaultRetryOptions()
        {
            return new RetryOptions(TimeSpan.FromMinutes(1), 3);
        }
    }
}
