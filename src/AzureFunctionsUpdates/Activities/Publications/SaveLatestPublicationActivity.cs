using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using AzureFunctionsUpdates.Models.Publications;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class SaveLatestPublicationActivity
    {
        [FunctionName(nameof(SaveLatestPublicationActivity))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<bool> Run(
            [ActivityTrigger] Publication publication,
            [Table(Configuration.Publications.TableName)]IAsyncCollector<Publication> collector,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestPublicationActivity)} for source: { publication.PublicationSourceName} and ID: { publication.Id}.");

            try
            {
                await collector.AddAsync(publication);
            }
            catch (Exception e)
            {
                logger.LogError($"Error when trying to save publication with " +
                                $"partitionKey: {publication.PartitionKey} and " +
                                $"rowKey: {publication.RowKey}.", e);
                
                return false;
            }

            return true;
        }
    }
}
