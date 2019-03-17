using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionsUpdates.Models.Publications
{
    public class LatestPublications
    {
        private IReadOnlyList<Publication> _latestPublicationsFromWeb;
        private IReadOnlyList<Publication> _latestPublicationsFromHistory;

        public LatestPublications(
            PublicationConfiguration publicationConfiguration,
            IReadOnlyList<Publication> latestReleasesFromGitHub,
            IReadOnlyList<Publication> latestReleasesFromHistory)
        {
            PublicationConfiguration = publicationConfiguration;
            _latestPublicationsFromWeb = latestReleasesFromGitHub;
            _latestPublicationsFromHistory = latestReleasesFromHistory;
        }

        public PublicationConfiguration PublicationConfiguration { get; set; }

        public Publication FromWeb => _latestPublicationsFromWeb.First(publication => publication.PublicationSourceName.Equals(PublicationConfiguration.PublicationSourceName, StringComparison.InvariantCultureIgnoreCase));

        public Publication FromHistory => _latestPublicationsFromHistory.First(publication => publication.PublicationSourceName.Equals(PublicationConfiguration.PublicationSourceName, StringComparison.InvariantCultureIgnoreCase));

        public bool IsNewAndShouldBeStored
        {
            get
            {
                if (FromWeb.GetType().Equals(typeof(NullPublication)))
                {
                    return false;
                }

                if (FromHistory.GetType().Equals(typeof(NullPublication)))
                {
                    return true;
                }

                return !(FromWeb.Id == FromHistory.Id);
            }
        }

        public bool IsNewAndShouldBePosted
        {
            get
            {
                return IsNewAndShouldBeStored && IsWithinTimeWindow();

                bool IsWithinTimeWindow()
                {
                    return DateTimeOffset.UtcNow.Subtract(FromWeb.PublicationDate).Days < MaximumNumberOfDaysToPostAboutNewlyFoundPublication;
                }
            }

            
        }

        public const int MaximumNumberOfDaysToPostAboutNewlyFoundPublication = 4;
    }
}
