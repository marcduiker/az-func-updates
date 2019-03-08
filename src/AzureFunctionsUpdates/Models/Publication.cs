using System;

namespace AzureFunctionsUpdates.Models
{
    public class Publication
    {
        public Publication()
        {}

        public Publication(
            string id,
            DateTimeOffset publicationDate,
            string title,
            string description,
            string url)
        {
            Id = id;
            PublicationDate = publicationDate;
            Title = title;
            Description = description;
            Url = url;
        }

        public string Id { get; set; }

        public DateTimeOffset PublicationDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        
    }
}
