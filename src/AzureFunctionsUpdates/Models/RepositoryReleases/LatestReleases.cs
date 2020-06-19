using System;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class LatestReleases
    {
        public LatestReleases()
        {}
        
        public LatestReleases(
            GitHubRepositoryRelease latestReleaseFromGitHub,
            HistoryRepositoryRelease latestReleaseFromHistory)
        {
            FromGitHub = latestReleaseFromGitHub;
            FromHistory = latestReleaseFromHistory;
        }

        public GitHubRepositoryRelease FromGitHub { get; }

        public HistoryRepositoryRelease FromHistory { get; }

        public bool IsNewAndShouldBeStored
        {
            get
            {
                if (string.IsNullOrEmpty(FromGitHub.PartitionKey))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(FromHistory.PartitionKey))
                {
                    return true;
                }

                return FromGitHub.ReleaseId != FromHistory.ReleaseId;
            }
        }

        public bool IsNewAndShouldBePosted
        {
            get
            {
                return IsNewAndShouldBeStored && IsWithinTimeWindow();

                bool IsWithinTimeWindow()
                {
                    return DateTimeOffset.UtcNow.Subtract(FromGitHub.ReleaseCreatedAt).Days 
                        < MaximumNumberOfDaysToPostAboutNewlyFoundRelease;
                }
            }
        }

        public bool IsSaved { get; set; }

        public const int MaximumNumberOfDaysToPostAboutNewlyFoundRelease = 7;
    }
}
