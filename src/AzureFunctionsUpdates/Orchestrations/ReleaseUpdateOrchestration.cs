using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Builders;
using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Activities.RepositoryReleases;
using AzureFunctionsUpdates.Models.RepositoryReleases;

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
            var repositoryConfigurations = await context.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                functionName: nameof(GetRepositoryConfigurations),
                retryOptions: GetDefaultRetryOptions(),
                input: null);

            if (repositoryConfigurations.Any())
            {
                var getLatestReleaseFromGitHubTasks = new List<Task<RepositoryRelease>>();
                var getLatestReleasesFromHistoryTasks = new List<Task<RepositoryRelease>>();

                // Fan out over the repos
                foreach (var repositoryConfiguration in repositoryConfigurations)
                {
                    // Get most recent release from GitHub
                    getLatestReleaseFromGitHubTasks.Add(context.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        GetDefaultRetryOptions(),
                        repositoryConfiguration));

                    // Get most recent known releases from history
                    getLatestReleasesFromHistoryTasks.Add(context.CallActivityWithRetryAsync<RepositoryRelease>(
                    nameof(GetLatestReleaseFromHistory),
                    GetDefaultRetryOptions(),
                    repositoryConfiguration));   
                }

                var latestFromGitHub = await Task.WhenAll(getLatestReleaseFromGitHubTasks);
                var latestFromHistory = await Task.WhenAll(getLatestReleasesFromHistoryTasks);
                var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();
                
                foreach (var repositoryConfiguration in repositoryConfigurations)
                {
                    var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                        repositoryConfiguration,
                        latestFromGitHub,
                        latestFromHistory,
                        releaseMatchFunction);
                    logger.LogInformation($"Repository: {repositoryConfiguration.RepositoryName} " +
                                          $"Tag: {latestReleases.FromGitHub.TagName}," +
                                          $"IsNewAndShouldBeStored: {latestReleases.IsNewAndShouldBeStored}, " +
                                          $"IsNewAndShouldBePosted: {latestReleases.IsNewAndShouldBePosted}.");
                    
                    if (latestReleases.IsNewAndShouldBeStored)
                    {
                        var isSaveSuccessful = await context.CallActivityWithRetryAsync<bool>(
                            nameof(SaveLatestRelease),
                            GetDefaultRetryOptions(),
                            latestReleases.FromGitHub);

                        if (isSaveSuccessful && Toggles.DoPostUpdate && latestReleases.IsNewAndShouldBePosted)
                        {
                            var message = MessageBuilder.BuildForRelease(latestReleases.FromGitHub);
                            try
                            {
                                await context.CallActivityWithRetryAsync(
                                  nameof(PostUpdate),
                                  GetDefaultRetryOptions(),
                                  message);
                            }
                            catch (FunctionFailedException ffe)
                            {
                                logger.LogError("Error when posting to Twitter", ffe);

                                await context.CallActivityAsync<UpdateMessage>(
                                    nameof(PostUpdateToDeadLetterQueue),
                                    message);
                            }
                        }
                    }
                }

                logger.LogInformation($"Completed {nameof(ReleaseUpdateOrchestration)}.");
            }
        }

        private static RetryOptions GetDefaultRetryOptions()
        {
            return new RetryOptions(TimeSpan.FromMinutes(1), 3);
        }
    }
}
