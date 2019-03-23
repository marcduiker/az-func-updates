using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class GetRepositoryConfigurations
    {
        [FunctionName(nameof(GetRepositoryConfigurations))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<IReadOnlyList<RepositoryConfiguration>> Run(
            [ActivityTrigger] string unusedInput,
            [Table(Configuration.RepositoryConfigurations.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetRepositoryConfigurations)}.");

            var configurations = new List<RepositoryConfiguration>();
            var query = QueryBuilder<RepositoryConfiguration>.CreateQueryForPartitionKey(
                Configuration.RepositoryConfigurations.PartitionKey);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            var activeConfigurations = queryResult.Results.Where(config => config.IsActive);
            configurations.AddRange(activeConfigurations);
            
            return configurations;
        }
    }
}
