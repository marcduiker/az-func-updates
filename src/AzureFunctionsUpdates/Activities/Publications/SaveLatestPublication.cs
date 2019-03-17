using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using AzureFunctionsUpdates.Models.Publications;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class SaveLatestPublication
    {
        [FunctionName(nameof(SaveLatestPublication))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task Run(
            [ActivityTrigger] Publication publication,
            [Table(Configuration.Publications.TableName)]IAsyncCollector<Publication> collector,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestPublication)} for source: { publication.PublicationSourceName} and ID: { publication.Id}.");

            await collector.AddAsync(publication);   
        }
    }
}
