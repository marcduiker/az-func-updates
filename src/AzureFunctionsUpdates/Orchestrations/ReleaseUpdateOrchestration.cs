using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Orchestrations
{
    public static class ReleaseUpdateOrchestration
    {
        [FunctionName(nameof(ReleaseUpdateOrchestration))]
        public static async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContextBase context,
            ILogger logger)
        {
            // Read repo links from storage table
            var repositories = await context.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                nameof(GetRepositoryConfigurations),
                GetDefaultRetryOptions(),
                null);

            if (repositories.Any())
            {
                var getLatestReleaseFromGitHubTasks = new List<Task<RepositoryRelease>>();
                var getLatestReleasesFromHistoryTasks = new List<Task<RepositoryRelease>>();

                // Fan out over the repos
                foreach (var repo in repositories)
                {
                    // Get most recent release from GitHub
                    getLatestReleaseFromGitHubTasks.Add(context.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        GetDefaultRetryOptions(),
                        repo));

                    // Get most recent known releases from history
                    getLatestReleasesFromHistoryTasks.Add(context.CallActivityWithRetryAsync<RepositoryRelease>(
                    nameof(GetLatestReleaseFromHistory),
                    GetDefaultRetryOptions(),
                    repo));   
                }

                var latestFromGitHub = await Task.WhenAll(getLatestReleaseFromGitHubTasks);
                var latestFromHistory = await Task.WhenAll(getLatestReleasesFromHistoryTasks);
               
                foreach (var repo in repositories)
                {
                    var latestReleases = new LatestReleases(repo, latestFromGitHub, latestFromHistory);
                    if (latestReleases.IsNewRelease)
                    {
                        await context.CallActivityWithRetryAsync(
                            nameof(SaveLatestRelease),
                            GetDefaultRetryOptions(),
                            latestReleases.FromGitHub);

                        await context.CallActivityWithRetryAsync(
                            nameof(PostUpdate),
                            GetDefaultRetryOptions(),
                            latestReleases.FromGitHub);
                    }
                }
            }
        }

        private static RetryOptions GetDefaultRetryOptions()
        {
            return new RetryOptions(TimeSpan.FromMinutes(1), 3);
        }
    }
}
