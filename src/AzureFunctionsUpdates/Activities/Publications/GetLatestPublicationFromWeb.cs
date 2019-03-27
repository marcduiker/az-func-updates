using AzureFunctionsUpdates.Models.Publications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed.Rss;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace AzureFunctionsUpdates.Activities.Publications
{
    public class GetLatestPublicationFromWeb
    {
        [FunctionName(nameof(GetLatestPublicationFromWeb))]
        public async Task<Publication> Run(
                [ActivityTrigger] PublicationConfiguration publicationConfiguration,
                ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestPublicationFromWeb)} for { publicationConfiguration.PublicationSourceOwner } { publicationConfiguration.PublicationSourceName}.");

            Publication publication = new NullPublication(publicationConfiguration.PublicationSourceName);

            using (var xmlReader = XmlReader.Create(publicationConfiguration.PublicationSourceUrl, new XmlReaderSettings() { Async = true }))
            {
                var parser = new RssParser();
                var reader = new RssFeedReader(xmlReader, parser);
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Item)
                    {
                        ISyndicationItem item = await reader.ReadItem();
                        publication = MapToPublication(publicationConfiguration, item);
                        break;
                    }
                }
            }
            
            logger.LogInformation($"Found publication on web for {publicationConfiguration.PublicationSourceName}: " +
                                  $"ID: {publication.Id}," +
                                  $"Date: {publication.PublicationDate:F}.");
        
            return publication;
        }

        private static Publication MapToPublication(PublicationConfiguration configuration, ISyndicationItem item)
        {
            return new Publication(
                publicationSourceName: configuration.PublicationSourceName,
                id: item.Id,
                publicationDate: item.Published,
                title: item.Title,
                description: item.Description,
                url: item.Links.FirstOrDefault()?.Uri.AbsoluteUri,
                hashTags: configuration.HashTags);
        }
    }
}
