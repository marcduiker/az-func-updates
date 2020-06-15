using AzureFunctionsUpdates.Models.Publications;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class GetPublicationConfigurationsActivity
    {
        [FunctionName(nameof(GetPublicationConfigurationsActivity))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<IReadOnlyList<PublicationConfiguration>> Run(
            [ActivityTrigger] string unusedInput,
            [Table(Configuration.RepositoryConfigurations.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetPublicationConfigurationsActivity)}.");

            var configurations = new List<PublicationConfiguration>();
            var query = QueryBuilder<PublicationConfiguration>.CreateQueryForPartitionKey(
                Configuration.PublicationConfigurations.PartitionKey);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            var activeConfigurations = queryResult.Results.Where(config => config.IsActive);
            configurations.AddRange(activeConfigurations);
            
            return configurations;
        }
    }
}
