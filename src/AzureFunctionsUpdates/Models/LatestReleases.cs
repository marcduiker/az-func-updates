using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionsUpdates.Models
{
    public class LatestReleases
    {
        private IReadOnlyList<RepositoryRelease> _latestReleasesFromGitHub;
        private IReadOnlyList<RepositoryRelease> _latestReleasesFromHistory;

        public LatestReleases(
            RepositoryConfiguration repository,
            IReadOnlyList<RepositoryRelease> latestReleasesFromGitHub,
            IReadOnlyList<RepositoryRelease> latestReleasesFromHistory)
        {
            Repository = repository;
            _latestReleasesFromGitHub = latestReleasesFromGitHub;
            _latestReleasesFromHistory = latestReleasesFromHistory;
        }

        public RepositoryConfiguration Repository { get; set; }

        public RepositoryRelease FromGitHub => _latestReleasesFromGitHub.First(release => release.RepositoryName.Equals(Repository.RepositoryName, StringComparison.InvariantCultureIgnoreCase));

        public RepositoryRelease FromHistory => _latestReleasesFromHistory.First(release => release.RepositoryName.Equals(Repository.RepositoryName, StringComparison.InvariantCultureIgnoreCase));

        public bool IsNewAndShouldBeStored
        {
            get
            {
                if (FromHistory.GetType().Equals(typeof(NullRelease)))
                {
                    return true;
                }

                if (FromGitHub.GetType().Equals(typeof(NullRelease)))
                {
                    return false;
                }

                return !(FromGitHub.ReleaseId == FromHistory.ReleaseId);
            }
        }

        public bool IsNewAndShouldBePosted
        {
            get
            {
                return IsNewAndShouldBeStored && IsWithinTimeWindow();

                bool IsWithinTimeWindow()
                {
                    return DateTimeOffset.UtcNow.Subtract(FromGitHub.ReleaseCreatedAt).Days < MaximumNumberOfDaysToPostAboutNewlyFoundRelease;
                }
            }

            
        }

        public const int MaximumNumberOfDaysToPostAboutNewlyFoundRelease = 4;
    }
}
