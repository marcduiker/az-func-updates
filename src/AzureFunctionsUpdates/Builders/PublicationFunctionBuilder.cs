using System;
using AzureFunctionsUpdates.Models.Publications;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.Builders
{
    public static class PublicationFunctionBuilder
    {
        public static Func<PublicationConfiguration, Publication, bool> BuildForMatchingPublicationSource()
        {
            return (config, publication) => publication.PublicationSourceName.Equals(config.PublicationSourceName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}