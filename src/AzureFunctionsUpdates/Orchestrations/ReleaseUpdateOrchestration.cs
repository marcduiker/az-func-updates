using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Builders;
using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            var repositoryConfigurations =
                await context.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                    functionName: nameof(GetRepositoryConfigurationsActivity),
                    retryOptions: RetryOptionsBuilder.BuildDefault(),
                    input: null);

            if (repositoryConfigurations.Any())
            {
                var releasesTasks = GetRepositoryReleasesTasks(context, repositoryConfigurations);
                var githubReleases = await Task.WhenAll(releasesTasks.GithubReleases);
                var historyReleases = await Task.WhenAll(releasesTasks.HistoryReleases);

                var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();

                foreach (var repositoryConfiguration in repositoryConfigurations)
                {
                    var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                            repositoryConfiguration,
                            githubReleases,
                            historyReleases,
                            releaseMatchFunction);
                    context.SetCustomStatus(repositoryConfiguration.RepositoryName);

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
            if (Toggles.DoPostUpdate && 
                latestReleases.IsSaved && 
                latestReleases.IsNewAndShouldBePosted)
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
        
        private static (List<Task<GitHubRepositoryRelease>> GithubReleases, List<Task<HistoryRepositoryRelease>> HistoryReleases) GetRepositoryReleasesTasks(
            IDurableOrchestrationContext context, 
            IReadOnlyList<RepositoryConfiguration> repositoryConfigurations)
        {
            var gitHubReleasesTasks = new List<Task<GitHubRepositoryRelease>>();
            var historyReleasesTasks = new List<Task<HistoryRepositoryRelease>>();

            // Fan out over the repos
            foreach (var repositoryConfiguration in repositoryConfigurations)
            {
                // Get most recent release from GitHub
                gitHubReleasesTasks.Add(context.CallActivityWithRetryAsync<GitHubRepositoryRelease>(
                    nameof(GetLatestReleaseFromGitHubActivity),
                    RetryOptionsBuilder.BuildDefault(),
                    repositoryConfiguration));

                // Get most recent known releases from history
                historyReleasesTasks.Add(context.CallActivityWithRetryAsync<HistoryRepositoryRelease>(
                    nameof(GetLatestReleaseFromHistoryActivity),
                    RetryOptionsBuilder.BuildDefault(),
                    repositoryConfiguration));
            }

            return (gitHubReleasesTasks, historyReleasesTasks);
        }

        private static IList<RepositoryRelease> GetReleasesOfTypes<TRelease, TNull>(RepositoryRelease[] repositoryReleases)
            where TRelease : RepositoryRelease
            where TNull : RepositoryRelease
        {
            return repositoryReleases.OfType<TRelease>()
                .Concat<RepositoryRelease>(repositoryReleases.OfType<TNull>())
                .ToList();
        }
    }
}