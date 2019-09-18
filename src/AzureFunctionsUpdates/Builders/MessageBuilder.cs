using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Models.Publications;
using System;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.Builders
{
    public static class MessageBuilder
    {
        public static UpdateMessage BuildForRelease(RepositoryRelease release)
        {
            string moreInfoAndHashTagContent = $"{Environment.NewLine}" +
                               $"{Environment.NewLine}" +
                               $"See {release.HtmlUrl} for more information." +
                               $"{Environment.NewLine}" +
                               $"{Environment.NewLine}" +
                               $"{release.HashTags}";

            int effectiveMoreInfoAndHashTagContentLength = moreInfoAndHashTagContent.Length - release.HtmlUrl.Length + TwitterShortenedUrlCharacterCount;
            int maxReleaseDescriptionLength = MaxTwitterCharacterCount - effectiveMoreInfoAndHashTagContentLength;
            
            string firstLine;
            if (string.IsNullOrEmpty(release.ReleaseName) || release.ReleaseName == release.TagName)
            {
                firstLine = GetReleaseDescriptionWithoutReleaseName(release);
            }
            else
            {
                firstLine = GetReleaseDescriptionWithReleaseName(release);
                if (firstLine.Length > maxReleaseDescriptionLength)
                {
                    firstLine = GetReleaseDescriptionWithoutReleaseName(release);
                }
            }

            var topic = $"{nameof(RepositoryRelease)}|{release.RepositoryName}";
            var content = firstLine + moreInfoAndHashTagContent;

            return new UpdateMessage(topic, content);
            
            string GetReleaseDescriptionWithoutReleaseName(RepositoryRelease repositoryRelease)
            {
                return $"A new {repositoryRelease.RepositoryName} release, tagged {repositoryRelease.TagName}, " +
                    $"is available on GitHub since {repositoryRelease.ReleaseCreatedAt:D}.";
            }
            
            string GetReleaseDescriptionWithReleaseName(RepositoryRelease repositoryRelease)
            {
                return $"A new {release.RepositoryName} release, {release.ReleaseName} (tagged {release.TagName}), " +
                       $"is available on GitHub since {release.ReleaseCreatedAt:D}.";
            }
        }

        public const int TwitterShortenedUrlCharacterCount = 28; // Urls are shortened to 28 characters by Twitter.
        public const int MaxTwitterCharacterCount = 255; // Urls are shortened to 28 characters by Twitter.

        public static UpdateMessage BuildForPublication(Publication publication)
        {
            var topic = $"{nameof(Publication)}|{publication.Title}";
            var content = $"A new {publication.PublicationSourceName} post titled '{publication.Title}' has been published on {publication.PublicationDate.ToString("D")}." +
                $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"See {publication.Url} to read the full announcement." +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"{publication.HashTags}";

            return new UpdateMessage(topic, content);
        }
    }
}
