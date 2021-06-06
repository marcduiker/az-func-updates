using AzureFunctionsUpdates.Models.RepositoryReleases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class SaveLatestReleaseActivity
    {
        [FunctionName(nameof(SaveLatestReleaseActivity))]
        [return: Table(Configuration.Releases.TableName, Connection = Configuration.ConnectionName)]
        public GitHubRepositoryRelease Run(
            [ActivityTrigger] GitHubRepositoryRelease repoRelease,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestReleaseActivity)} for " +
                $"{ repoRelease.RepositoryName} " +
                $"{ repoRelease.ReleaseName}.");

            return repoRelease;
        }
    }
}
