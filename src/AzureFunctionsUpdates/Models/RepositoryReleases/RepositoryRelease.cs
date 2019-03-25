using System;
using AzureFunctionsUpdates.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class RepositoryRelease : TableEntity
    {
        public RepositoryRelease()
        {}

        public RepositoryRelease(
            string repositoryName,
            int releaseId,
            string releaseName,
            string tagName,
            DateTimeOffset createdAt,
            string htmlUrl,
            string body,
            string hashTags
            )
        {
            PartitionKey = KeyFormatter.SanitizeKey(repositoryName);
            RowKey = KeyFormatter.SanitizeKey($"{releaseId.ToString()}-{tagName}");

            ReleaseId = releaseId;
            RepositoryName = repositoryName;
            ReleaseName = releaseName;
            TagName = tagName;
            ReleaseCreatedAt = createdAt;
            HtmlUrl = htmlUrl;
            Body = body;
            HashTags = hashTags;
            CreatedAt = DateTimeOffset.Now;
        }

        public int ReleaseId { get; set; }

        public string RepositoryName { get; set; }

        public string ReleaseName { get; set; }

        public string TagName { get; set; }

        public DateTimeOffset ReleaseCreatedAt { get; set; }

        public string HtmlUrl { get; set; }

        public string Body { get; set; }

        public string HashTags { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
