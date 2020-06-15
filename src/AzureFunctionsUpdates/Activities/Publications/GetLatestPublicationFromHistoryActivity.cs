using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using AzureFunctionsUpdates.Models.Publications;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class GetLatestPublicationFromHistoryActivity
    {
        [FunctionName(nameof(GetLatestPublicationFromHistoryActivity))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<Publication> Run(
            [ActivityTrigger] PublicationConfiguration publicationConfiguration,
            [Table(Configuration.Publications.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestPublicationFromHistoryActivity)} for { publicationConfiguration.PublicationSourceOwner } { publicationConfiguration.PublicationSourceName }.");

            Publication latestKnownPublication = null;
            var query = QueryBuilder<Publication>.CreateQueryForPartitionKey(publicationConfiguration.PublicationSourceName);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            latestKnownPublication = queryResult.Results.AsReadOnly().OrderByDescending(publication => publication.PublicationDate).FirstOrDefault();

            if (latestKnownPublication != null)
            {
                logger.LogInformation($"Found publication in history for configuration: {publicationConfiguration.PublicationSourceName}, " +
                                      $"ID: {latestKnownPublication.Id}," +
                                      $"PublicationDate: {latestKnownPublication.PublicationDate:F}.");
            }
            
            return latestKnownPublication ?? new NullPublication(publicationConfiguration.PublicationSourceName);
        }
    }
}
