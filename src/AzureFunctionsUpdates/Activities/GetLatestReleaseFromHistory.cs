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
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromHistory)} for { repoConfiguration.RepositoryOwner } { repoConfiguration.RepositoryName }.");

            RepositoryRelease latestKnownRelease = null;            
            var query = QueryBuilder<RepositoryRelease>.CreateQueryForPartitionKey(repoConfiguration.RepositoryName);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            latestKnownRelease = queryResult.Results.AsReadOnly().OrderByDescending(release => release.CreatedAt).FirstOrDefault();

            return latestKnownRelease ?? new NullRelease(repoConfiguration.RepositoryName);
        }
    }
}
