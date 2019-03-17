using AzureFunctionsUpdates.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureFunctionsUpdates.Models.Releases
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
