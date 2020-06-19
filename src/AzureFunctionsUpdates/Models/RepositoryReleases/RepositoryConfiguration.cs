using System;
using AzureFunctionsUpdates.Storage;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class RepositoryConfiguration : BaseConfiguration
    {
        public RepositoryConfiguration()
        {}

        public RepositoryConfiguration(
            string repositoryOwner,
            string repositoryName,
            string hashTags)
        {
            PartitionKey = KeyFormatter.SanitizeKey(Configuration.RepositoryConfigurations.PartitionKey);
            RowKey = KeyFormatter.SanitizeKey($"{repositoryOwner}|{repositoryName}");

            RepositoryOwner = repositoryOwner;
            RepositoryName = repositoryName;
            HashTags = hashTags;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public string RepositoryOwner { get; set; }

        public string RepositoryName { get; set; }
    }
}
