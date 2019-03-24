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
            PartitionKey = Configuration.RepositoryConfigurations.PartitionKey;
            RowKey = $"{repositoryOwner}|{repositoryName}";

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
