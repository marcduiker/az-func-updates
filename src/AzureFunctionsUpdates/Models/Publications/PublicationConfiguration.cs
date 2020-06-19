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
            PartitionKey = KeyFormatter.SanitizeKey(Configuration.PublicationConfigurations.PartitionKey);
            RowKey = KeyFormatter.SanitizeKey($"{publicationSourceOwner}|{publicationSourceName}");

            PublicationSourceOwner = publicationSourceOwner;
            PublicationSourceName = publicationSourceName;
            PublicationSourceUrl = publicationUrl;
            HashTags = hashTags;
            IsActive = true;
        }

        public string PublicationSourceOwner { get; set; }

        public string PublicationSourceName { get; set; }

        public string PublicationSourceUrl { get; set; }
    }
}
