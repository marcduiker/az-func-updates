using AzureFunctionsUpdates.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureFunctionsUpdates.Models
{
    public class RepositoryConfiguration : TableEntity
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
        }

        public string RepositoryOwner { get; set; }

        public string RepositoryName { get; set; }

        public string HashTags { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
