using AzureFunctionsUpdates.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsUpdates.Models
{
    public class PublicationConfiguration : TableEntity
    {
        public PublicationConfiguration()
        {}

        public PublicationConfiguration(
            string url,
            string hashTags)
        {
            PartitionKey = Configuration.PublicationConfigurations.PartitionKey;
            RowKey = url;

            Url = url;
            HashTags = hashTags;
            IsActive = true;
        }

        public string Url { get; set; }

        public string HashTags { get; set; }

        public bool IsActive { get; set; }
    }
}
