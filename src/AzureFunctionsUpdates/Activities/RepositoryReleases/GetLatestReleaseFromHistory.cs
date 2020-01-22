using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class GetLatestReleaseFromHistory
    {
        [FunctionName(nameof(GetLatestReleaseFromHistory))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<RepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            [Table(Configuration.Releases.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromHistory)} for " +
                $"{ repoConfiguration.RepositoryOwner } " +
                $"{ repoConfiguration.RepositoryName }.");

            RepositoryRelease latestKnownRelease = null;
            var query = QueryBuilder<RepositoryRelease>.CreateQueryForPartitionKey(repoConfiguration.RepositoryName);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            latestKnownRelease = queryResult.Results.AsReadOnly().OrderByDescending(release => release.CreatedAt).FirstOrDefault();
            
            if (latestKnownRelease != null)
            {
                logger.LogInformation($"Found release in history for configuration: {repoConfiguration.RepositoryName}, " +
                                      $"Release ID: {latestKnownRelease.ReleaseId}," +
                                      $"ReleaseCreatedAt: {latestKnownRelease.ReleaseCreatedAt:F}.");
            }
            
            return latestKnownRelease ?? new NullRelease(repoConfiguration.RepositoryName);
        }
    }
}
