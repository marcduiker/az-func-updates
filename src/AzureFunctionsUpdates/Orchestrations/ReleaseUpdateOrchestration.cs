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
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.Orchestrations
{
    public class ReleaseUpdateOrchestration
    {
        [FunctionName(nameof(ReleaseUpdateOrchestration))]
        public async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            // Read repo links from storage table
            var repositoryConfigurations =
                await context.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                    functionName: nameof(GetRepositoryConfigurationsActivity),
                    retryOptions: RetryOptionsBuilder.BuildDefault(),
                    input: null);

            if (repositoryConfigurations.Any())
            {
                var repositoryReleasesTasks = GetRepositoryReleasesTasks(context, repositoryConfigurations);
                var repositoryReleases = await Task.WhenAll(repositoryReleasesTasks);

                var latestFromGitHub =  GetReleasesOfTypes<GitHubRepositoryRelease, GitHubNullRelease>(repositoryReleases);
                var latestFromHistory =  GetReleasesOfTypes<HistoryRepositoryRelease, HistoryNullRelease>(repositoryReleases);

                var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();

                foreach (var repositoryConfiguration in repositoryConfigurations)
                {
                    var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                            repositoryConfiguration,
                            latestFromGitHub,
                            latestFromHistory,
                            releaseMatchFunction);
                    LogLatestReleases(logger, repositoryConfiguration, latestReleases);

                    latestReleases.IsSaved = await SaveLatestRelease(context, logger, latestReleases);
                    await PostLatestRelease(context, logger, latestReleases);
                }
            }
        }

        private static async Task<bool> SaveLatestRelease(
            IDurableOrchestrationContext context,
            ILogger logger,
            LatestReleases latestReleases)
        {
            if (latestReleases.IsNewAndShouldBeStored)
            {
                try
                {
                    await context.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(SaveLatestReleaseActivity),
                        RetryOptionsBuilder.BuildDefault(),
                        latestReleases.FromGitHub);
                }
                catch (FunctionFailedException ffe)
                {
                    logger.LogError("Error when saving repositoryRelease", ffe, latestReleases);
                    return false;
                }
            }

            return true;
        }

        private static async Task PostLatestRelease(
            IDurableOrchestrationContext context,
            ILogger logger,
            LatestReleases latestReleases)
        {
            if (Toggles.DoPostUpdate && latestReleases.IsSaved && latestReleases.IsNewAndShouldBePosted)
            {
                var message = MessageBuilder.BuildForRelease(latestReleases.FromGitHub);
                try
                {
                    await context.CallActivityWithRetryAsync<bool>(
                        nameof(PostUpdateActivity),
                        RetryOptionsBuilder.BuildDefault(),
                        message);
                }
                catch (FunctionFailedException ffe)
                {
                    logger.LogError("Error when posting to Twitter", ffe);

                    await context.CallActivityAsync<UpdateMessage>(
                        nameof(PostUpdateToDeadLetterQueueActivity),
                        message);
                }
            }
        }
        
        private static List<Task<RepositoryRelease>> GetRepositoryReleasesTasks(
            IDurableOrchestrationContext context, 
            IReadOnlyList<RepositoryConfiguration> repositoryConfigurations)
        {
            var getLatestReleasesTasks = new List<Task<RepositoryRelease>>();
            
            // Fan out over the repos
            foreach (var repositoryConfiguration in repositoryConfigurations)
            {
                // Get most recent release from GitHub
                getLatestReleasesTasks.Add(context.CallActivityWithRetryAsync<RepositoryRelease>(
                    nameof(GetLatestReleaseFromGitHubActivity),
                    RetryOptionsBuilder.BuildDefault(),
                    repositoryConfiguration));

                // Get most recent known releases from history
                getLatestReleasesTasks.Add(context.CallActivityWithRetryAsync<RepositoryRelease>(
                    nameof(GetLatestReleaseFromHistoryActivity),
                    RetryOptionsBuilder.BuildDefault(),
                    repositoryConfiguration));
            }

            return getLatestReleasesTasks;
        }

        private static IList<RepositoryRelease> GetReleasesOfTypes<TRelease, TNull>(RepositoryRelease[] repositoryReleases)
            where TRelease : RepositoryRelease
            where TNull : RepositoryRelease
        {
            return repositoryReleases.OfType<TRelease>()
                .Concat<RepositoryRelease>(repositoryReleases.OfType<TNull>())
                .ToList();
        }

        private static void LogLatestReleases(
            ILogger logger, 
            RepositoryConfiguration repositoryConfiguration,
            LatestReleases latestReleases)
        {
            logger.LogInformation("Repository={repositoryName} " +
                                  "Tag={tagName}," +
                                  "IsNewAndShouldBeStored={isNewAndShouldBeStored}, " +
                                  "IsNewAndShouldBePosted={isNewAndShouldBePosted}.",
                repositoryConfiguration.RepositoryName,
                latestReleases.FromGitHub.TagName,
                latestReleases.IsNewAndShouldBeStored,
                latestReleases.IsNewAndShouldBePosted);
        }
    }
}