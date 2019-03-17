using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Activities.Releases;
using AzureFunctionsUpdates.Builders;
using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Models.Releases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Orchestrations
{
    public class ReleaseUpdateOrchestration
    {
        [FunctionName(nameof(ReleaseUpdateOrchestration))]
        public async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContextBase context,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(ReleaseUpdateOrchestration)}.");
            
            // Read repo links from storage table
            var repositories = await context.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                functionName: nameof(GetRepositoryConfigurations),
                retryOptions: GetDefaultRetryOptions(),
                input: null);

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

                var saveAndUpdateTasks = new List<Task>();
                foreach (var repo in repositories)
                {
                    var latestReleases = new LatestReleases(repo, latestFromGitHub, latestFromHistory);
                    if (latestReleases.IsNewAndShouldBeStored)
                    {
                        saveAndUpdateTasks.Add(context.CallActivityWithRetryAsync(
                            nameof(SaveLatestRelease),
                            GetDefaultRetryOptions(),
                            latestReleases.FromGitHub));

                        if (Toggles.DoPostUpdate)
                        {
                            var message = MessageBuilder.BuildForRelease(latestReleases.FromGitHub);
                            saveAndUpdateTasks.Add(context.CallActivityWithRetryAsync(
                                  nameof(PostUpdate),
                                  GetDefaultRetryOptions(),
                                  message));
                        }
                    }
                }

                await Task.WhenAll(saveAndUpdateTasks);
                logger.LogInformation($"Completed {nameof(ReleaseUpdateOrchestration)}.");
            }
        }

        private static RetryOptions GetDefaultRetryOptions()
        {
            return new RetryOptions(TimeSpan.FromMinutes(1), 3);
        }
    }
}
