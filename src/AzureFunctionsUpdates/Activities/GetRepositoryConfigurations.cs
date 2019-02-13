using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities
{
    public static class GetRepositoryConfigurations
    {
        [FunctionName(nameof(GetRepositoryConfigurations))]
        [StorageAccount(Configuration.ConnectionName)]
        public static async Task<IReadOnlyList<RepositoryConfiguration>> Run(
            [ActivityTrigger] string unusedInput,
            [Table(Configuration.RepositoryConfigurations.TableName)] CloudTable table,
            ILogger logger)
        {
            var repositoryConfigurations = new List<RepositoryConfiguration>();
            try
            {
                var query = QueryBuilder<RepositoryConfiguration>.CreateQueryForPartitionKey(Configuration.RepositoryConfigurations.PartitionKey);
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
                repositoryConfigurations.AddRange(queryResult);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to read repository configurations from {Configuration.RepositoryConfigurations.TableName}.", e);
            }

            return repositoryConfigurations;
        }
    }
}
