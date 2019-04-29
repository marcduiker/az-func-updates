using System;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class SaveLatestRelease
    {
        [FunctionName(nameof(SaveLatestRelease))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<bool> Run(
            [ActivityTrigger] RepositoryRelease repoRelease,
            [Table(Configuration.Releases.TableName)]IAsyncCollector<RepositoryRelease> collector,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestRelease)} for " +
                $"{ repoRelease.RepositoryName} " +
                $"{ repoRelease.ReleaseName}.");

            try
            {
                await collector.AddAsync(repoRelease);
            }
            catch (Exception e)
            {
                logger.LogError($"Error when trying to save repository release with " +
                    $"partitionKey: {repoRelease.PartitionKey} and " +
                    $"rowKey: {repoRelease.RowKey}.", e);
                
                return false;
            }

            return true;  
        }
    }
}
