using AzureFunctionsUpdates.Models.Publications;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class GetPublicationConfigurations
    {
        [FunctionName(nameof(GetPublicationConfigurations))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<IReadOnlyList<PublicationConfiguration>> Run(
            [ActivityTrigger] string unusedInput,
            [Table(Configuration.RepositoryConfigurations.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetPublicationConfigurations)}.");

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
