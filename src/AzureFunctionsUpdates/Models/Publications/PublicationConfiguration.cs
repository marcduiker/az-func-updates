using AzureFunctionsUpdates.Storage;

namespace AzureFunctionsUpdates.Models.Publications
{
    public class PublicationConfiguration : BaseConfiguration
    {
        public PublicationConfiguration()
        {}

        public PublicationConfiguration(
            string publicationSourceOwner,
            string publicationSourceName,
            string publicationUrl,
            string hashTags)
        {
            PartitionKey = Configuration.PublicationConfigurations.PartitionKey;
            RowKey = $"{publicationSourceOwner}|{publicationSourceName}";

            PublicationSourceOwner = publicationSourceOwner;
            PublicationSourceName = publicationSourceName;
            PublicationUrl = publicationUrl;
            HashTags = hashTags;
            IsActive = true;
        }

        public string PublicationSourceOwner { get; set; }

        public string PublicationSourceName { get; set; }

        public string PublicationUrl { get; set; }
    }
}
