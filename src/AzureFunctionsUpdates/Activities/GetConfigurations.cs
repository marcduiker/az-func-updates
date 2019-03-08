using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities
{
    public class GetConfigurations
    {
        [FunctionName(nameof(GetConfigurations))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<IReadOnlyList<RepositoryConfiguration>> Run(
            [ActivityTrigger] string unusedInput,
            [Table(Configuration.RepositoryConfigurations.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetConfigurations)}.");

            var repositoryConfigurations = new List<RepositoryConfiguration>();
            var query = QueryBuilder<RepositoryConfiguration>.CreateQueryForPartitionKey(Configuration.RepositoryConfigurations.PartitionKey);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            var activeConfigurations = queryResult.Results.Where(config => config.IsActive);
            repositoryConfigurations.AddRange(activeConfigurations);
            
            return repositoryConfigurations;
        }
    }
}
