using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class LatestReleases
    {
        public LatestReleases()
        {}
        
        public LatestReleases(
            RepositoryRelease latestReleaseFromGitHub,
            RepositoryRelease latestReleaseFromHistory)
        {
            FromGitHub = latestReleaseFromGitHub;
            FromHistory = latestReleaseFromHistory;
        }

        public RepositoryRelease FromGitHub { get; }

        public RepositoryRelease FromHistory { get; }

        public bool IsNewAndShouldBeStored
        {
            get
            {
                if (FromGitHub.GetType() == typeof(NullRelease))
                {
                    return false;
                }

                if (FromHistory.GetType() == typeof(NullRelease))
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

        public const int MaximumNumberOfDaysToPostAboutNewlyFoundRelease = 3;
    }
}
