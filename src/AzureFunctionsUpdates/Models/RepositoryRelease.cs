using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace AzureFunctionsUpdates.Models
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
            string body
            )
        {
            PartitionKey = repositoryName;
            RowKey = releaseId.ToString();

            ReleaseId = releaseId;
            RepositoryName = repositoryName;
            ReleaseName = releaseName;
            TagName = tagName;
            ReleaseCreatedAt = createdAt;
            HtmlUrl = htmlUrl;
            Body = body;
            CreatedAt = DateTimeOffset.Now;
        }

        public int ReleaseId { get; set; }

        public string RepositoryName { get; set; }

        public string ReleaseName { get; set; }

        public string TagName { get; set; }

        public DateTimeOffset ReleaseCreatedAt { get; set; }

        public string HtmlUrl { get; set; }

        public string Body { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
