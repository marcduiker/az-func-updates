using System;

namespace AzureFunctionsUpdates.Models.Publications
{
    public class LatestPublications
    {
        public LatestPublications()
        {}
        
        public LatestPublications(
            
            Publication fromWeb,
            Publication fromHistory)
        {
            FromWeb = fromWeb;
            FromHistory = fromHistory;
        }     

        public Publication FromWeb { get; }

        public Publication FromHistory { get; }

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
