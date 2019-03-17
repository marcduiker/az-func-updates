using AzureFunctionsUpdates.Models.Publications;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
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

            using (var xmlReader = XmlReader.Create(publicationConfiguration.PublicationUrl, new XmlReaderSettings() { Async = true }))
            {
                var reader = new AtomFeedReader(xmlReader);
                while (await reader.Read())
                {
                    if (reader.ElementType == SyndicationElementType.Item)
                    {
                        IAtomEntry entry = await reader.ReadEntry();
                        publication = MapToPublication(publicationConfiguration, entry);
                        break;
                    }
                }
            }
        
            return publication;
        }

        private static Publication MapToPublication(PublicationConfiguration configuration, IAtomEntry atomEntry)
        {
            return new Publication(
                publicationSourceName: configuration.PublicationSourceName,
                id: atomEntry.Id,
                publicationDate: atomEntry.Published,
                title: atomEntry.Title,
                description: atomEntry.Description,
                url: atomEntry.Links.FirstOrDefault()?.Uri.AbsolutePath);
        }
    }
}
