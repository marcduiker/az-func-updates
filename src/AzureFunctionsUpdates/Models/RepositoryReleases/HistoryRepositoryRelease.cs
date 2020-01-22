using System;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class HistoryRepositoryRelease : RepositoryRelease
    {
        public HistoryRepositoryRelease() : base()
        {}

        public HistoryRepositoryRelease(
            string repositoryName,
            int releaseId,
            string releaseName,
            string tagName,
            DateTimeOffset createdAt,
            string htmlUrl,
            string body,
            string hashTags
            )
            : base(
                  repositoryName,
                  releaseId,
                  releaseName,
                  tagName,
                  createdAt,
                  htmlUrl,
                  body,
                  hashTags)
        {
        }
    }
}
