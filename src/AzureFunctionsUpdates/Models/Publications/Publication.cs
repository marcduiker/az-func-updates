using Microsoft.WindowsAzure.Storage.Table;
using System;

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
            string url)
        {
            PublicationSourceName = publicationSourceName;
            Id = id;
            PublicationDate = publicationDate;
            Title = title;
            Description = description;
            Url = url;
        }

        public string PublicationSourceName { get; set; }

        public string Id { get; set; }

        public DateTimeOffset PublicationDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        
    }
}
