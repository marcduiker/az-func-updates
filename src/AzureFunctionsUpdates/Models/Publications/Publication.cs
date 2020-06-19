using System;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctionsUpdates.Models.Publications
{
    public class Publication : TableEntity
    {
        public Publication()
        {}

        public Publication(
            string publicationSourceName,
            string id,
            DateTimeOffset publicationDate,
            string title,
            string description,
            string url,
            string hashTags)
        {
            PartitionKey = KeyFormatter.SanitizeKey(publicationSourceName);
            PublicationSourceName = publicationSourceName;
            RowKey = KeyFormatter.SanitizeKey($"{id}-{publicationDate.ToUnixTimeSeconds()}");
            Id = id;
            PublicationDate = publicationDate;
            Title = title;
            Description = description;
            Url = url;
            HashTags = hashTags;
        }

        public string PublicationSourceName { get; set; }

        public string Id { get; set; }

        public DateTimeOffset PublicationDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string HashTags { get; set; }
    }
}
