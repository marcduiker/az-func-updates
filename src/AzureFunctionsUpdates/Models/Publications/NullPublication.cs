namespace AzureFunctionsUpdates.Models.Publications
{
    public class NullPublication : Publication
    {
        public NullPublication(string publicationSourceName) :
            base (
                publicationSourceName: publicationSourceName,
                id: default,
                publicationDate: default,
                title: default,
                description: default,
                url: default
                )
        {}
    }
}