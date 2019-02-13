using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsUpdates.Activities
{
    public static class GetLatestReleaseFromHistory
    {
        [FunctionName(nameof(GetLatestReleaseFromHistory))]
        [StorageAccount(Configuration.ConnectionName)]
        public static async Task<RepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            [Table(Configuration.Releases.TableName)] CloudTable table,
            ILogger logger)
        {
            RepositoryRelease latestKnownRelease = null;
            try
            {
                var query = QueryBuilder<RepositoryRelease>.CreateQueryForPartitionKey(repoConfiguration.RepositoryName);
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
                latestKnownRelease = queryResult.Results.AsReadOnly().OrderByDescending(release => release.CreatedAt).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to retrieve release history for {repoConfiguration.RepositoryOwner}/{repoConfiguration.RepositoryName} from {Configuration.Releases.TableName}.", e);
            }

            return latestKnownRelease ?? new NullRelease(repoConfiguration.RepositoryName);
        }
    }
}
