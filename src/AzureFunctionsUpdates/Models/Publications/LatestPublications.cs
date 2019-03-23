using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionsUpdates.Models.Publications
{
    public class LatestPublications
    {
        private readonly IReadOnlyList<Publication> _latestPublicationsFromWeb;
        private readonly IReadOnlyList<Publication> _latestPublicationsFromHistory;
        private readonly PublicationConfiguration _publicationConfiguration;

        public LatestPublications(
            PublicationConfiguration publicationConfiguration,
            IReadOnlyList<Publication> latestReleasesFromGitHub,
            IReadOnlyList<Publication> latestReleasesFromHistory)
        {
            this._publicationConfiguration = publicationConfiguration;
            _latestPublicationsFromWeb = latestReleasesFromGitHub;
            _latestPublicationsFromHistory = latestReleasesFromHistory;
        }

        

        public Publication FromWeb => _latestPublicationsFromWeb.First(publication => publication.PublicationSourceName.Equals(_publicationConfiguration.PublicationSourceName, StringComparison.InvariantCultureIgnoreCase));

        public Publication FromHistory => _latestPublicationsFromHistory.First(publication => publication.PublicationSourceName.Equals(_publicationConfiguration.PublicationSourceName, StringComparison.InvariantCultureIgnoreCase));

        public bool IsNewAndShouldBeStored
        {
            get
            {
                if (FromWeb.GetType() == typeof(NullPublication))
                {
                    return false;
                }

                if (FromHistory.GetType() == typeof(NullPublication))
                {
                    return true;
                }

                return FromWeb.Id != FromHistory.Id;
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

        public const int MaximumNumberOfDaysToPostAboutNewlyFoundPublication = 2;
    }
}
