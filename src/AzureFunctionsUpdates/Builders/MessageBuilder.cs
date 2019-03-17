using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Models.Publications;
using AzureFunctionsUpdates.Models.Releases;
using System;

namespace AzureFunctionsUpdates.Builders
{
    public static class MessageBuilder
    {
        public static UpdateMessage BuildForRelease(RepositoryRelease release)
        {
            string firstLine;
            if (string.IsNullOrEmpty(release.ReleaseName) || release.ReleaseName == release.TagName)
            {
                firstLine = $"A new {release.RepositoryName} release, tagged {release.TagName}, is available on GitHub since {release.ReleaseCreatedAt.ToString("D")}.";
            }
            else
            {
                firstLine = $"A new {release.RepositoryName} release, {release.ReleaseName} (tagged {release.TagName}), is available on GitHub since {release.ReleaseCreatedAt.ToString("D")}.";
            }

            var topic = $"{nameof(RepositoryRelease)}|{release.RepositoryName}";
            var content = firstLine +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"See {release.HtmlUrl} for more information." +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"{release.HashTags}";

            return new UpdateMessage(topic, content);
        }

        public static UpdateMessage BuildForPublication(Publication publication)
        {
            var topic = $"{nameof(Publication)}|{publication.Title}";
            var content = $"A new {publication.PublicationSourceName} post titled '{publication.Title}' has been published on {publication.PublicationDate.ToString("D")}." +
                $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"See {publication.Url} to read the full post." +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"{publication.HashTags}";

            return new UpdateMessage(topic, content);
        }
    }
}
