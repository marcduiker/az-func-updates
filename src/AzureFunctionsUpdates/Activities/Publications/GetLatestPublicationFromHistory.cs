using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using AzureFunctionsUpdates.Models.Publications;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class GetLatestPublicationFromHistory
    {
        [FunctionName(nameof(GetLatestPublicationFromHistory))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task<Publication> Run(
            [ActivityTrigger] PublicationConfiguration publicationConfiguration,
            [Table(Configuration.Releases.TableName)] CloudTable table,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestPublicationFromHistory)} for { publicationConfiguration.PublicationSourceOwner } { publicationConfiguration.PublicationSourceName }.");

            Publication latestKnownPublication = null;            
            var query = QueryBuilder<Publication>.CreateQueryForPartitionKey(publicationConfiguration.PublicationSourceName);
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);
            latestKnownPublication = queryResult.Results.AsReadOnly().OrderByDescending(publication => publication.PublicationDate).FirstOrDefault();

            return latestKnownPublication ?? new NullPublication(publicationConfiguration.PublicationSourceName);
        }
    }
}
