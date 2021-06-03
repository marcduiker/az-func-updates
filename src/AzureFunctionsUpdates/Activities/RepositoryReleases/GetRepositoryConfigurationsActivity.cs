﻿using System.Collections.Generic;
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
    public class GetRepositoryConfigurationsActivity
    {
        [FunctionName(nameof(GetRepositoryConfigurationsActivity))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<IReadOnlyList<RepositoryConfiguration>> Run(
            [ActivityTrigger] string unusedInput,
            [Table(Configuration.RepositoryConfigurations.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetRepositoryConfigurationsActivity)}.");

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
