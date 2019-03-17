using System;

namespace AzureFunctionsUpdates.Models.Publications
{
    public class NullPublication : Publication
    {
        public NullPublication(string publicationSourceName) :
            base (
                publicationSourceName: publicationSourceName,
                id: default(string),
                publicationDate: default(DateTimeOffset),
                title: default(string),
                description: default(string),
                url: default(string),
                hashTags: default(string)
                )
        {}
    }
}