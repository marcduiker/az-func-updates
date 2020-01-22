using System;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class GitHubRepositoryRelease : RepositoryRelease
    {
        public GitHubRepositoryRelease() : base()
        {}

        public GitHubRepositoryRelease(
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
