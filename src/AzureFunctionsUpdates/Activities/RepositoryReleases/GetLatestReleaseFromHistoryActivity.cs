using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class GetLatestReleaseFromHistoryActivity
    {
        [FunctionName(nameof(GetLatestReleaseFromHistoryActivity))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<HistoryRepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            [Table(Configuration.Releases.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromHistoryActivity)} for " +
                $"{ repoConfiguration.RepositoryOwner } " +
                $"{ repoConfiguration.RepositoryName }.");

            HistoryRepositoryRelease latestKnownRelease = null;
            var query = QueryBuilder<HistoryRepositoryRelease>.CreateQueryForPartitionKey(repoConfiguration.RepositoryName);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            latestKnownRelease = queryResult.Results.AsReadOnly().OrderByDescending(release => release.CreatedAt).FirstOrDefault();
            
            if (latestKnownRelease != null)
            {
                logger.LogInformation($"Found release in history for configuration: {repoConfiguration.RepositoryName}, " +
                                      $"Release ID: {latestKnownRelease.ReleaseId}," +
                                      $"ReleaseCreatedAt: {latestKnownRelease.ReleaseCreatedAt:F}.");
            }
            
            return latestKnownRelease ?? new HistoryRepositoryRelease(repoConfiguration.RepositoryName);
        }
    }
}
