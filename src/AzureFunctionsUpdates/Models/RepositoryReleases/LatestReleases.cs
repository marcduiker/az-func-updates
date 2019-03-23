using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class LatestReleases
    {
        private readonly IReadOnlyList<RepositoryRelease> _latestReleasesFromGitHub;
        private readonly IReadOnlyList<RepositoryRelease> _latestReleasesFromHistory;
        private readonly RepositoryConfiguration _repository;
        
        public LatestReleases(
            RepositoryConfiguration repository,
            IReadOnlyList<RepositoryRelease> latestReleasesFromGitHub,
            IReadOnlyList<RepositoryRelease> latestReleasesFromHistory)
        {
            _repository = repository;
            _latestReleasesFromGitHub = latestReleasesFromGitHub;
            _latestReleasesFromHistory = latestReleasesFromHistory;
        }

        public RepositoryRelease FromGitHub => _latestReleasesFromGitHub.First(release => release.RepositoryName.Equals(_repository.RepositoryName, StringComparison.InvariantCultureIgnoreCase));

        public RepositoryRelease FromHistory => _latestReleasesFromHistory.First(release => release.RepositoryName.Equals(_repository.RepositoryName, StringComparison.InvariantCultureIgnoreCase));

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
                    return DateTimeOffset.UtcNow.Subtract(FromGitHub.ReleaseCreatedAt).Days < MaximumNumberOfDaysToPostAboutNewlyFoundRelease;
                }
            }

            
        }

        public const int MaximumNumberOfDaysToPostAboutNewlyFoundRelease = 2;
    }
}
