using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using AzureFunctionsUpdates.Models.Releases;

namespace AzureFunctionsUpdates.Activities.Releases
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
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromHistory)} for { repoConfiguration.RepositoryOwner } { repoConfiguration.RepositoryName }.");

            RepositoryRelease latestKnownRelease = null;            
            var query = QueryBuilder<RepositoryRelease>.CreateQueryForPartitionKey(repoConfiguration.RepositoryName);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            latestKnownRelease = queryResult.Results.AsReadOnly().OrderByDescending(release => release.CreatedAt).FirstOrDefault();

            return latestKnownRelease ?? new NullRelease(repoConfiguration.RepositoryName);
        }
    }
}
