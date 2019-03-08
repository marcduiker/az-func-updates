using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using Tweetinvi;
using Tweetinvi.Models;

namespace AzureFunctionsUpdates.Activities
{
    public class PostUpdate
    {
        private readonly string consumerApiKey = Environment.GetEnvironmentVariable("Twitter_Consumer_Api_Key");
        private readonly string consumerApiSecret = Environment.GetEnvironmentVariable("Twitter_Consumer_Api_Secret");
        private readonly string accessToken = Environment.GetEnvironmentVariable("Twitter_Access_Token");
        private readonly string accessTokenSecret = Environment.GetEnvironmentVariable("Twitter_Access_Token_Secret");

        [FunctionName(nameof(PostUpdate))]
        public void Run(
            [ActivityTrigger] RepositoryRelease newRelease,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(PostUpdate)} for { newRelease.RepositoryName} { newRelease.ReleaseName}.");

            var creds = new TwitterCredentials(consumerApiKey, consumerApiSecret, accessToken, accessTokenSecret);
            var tweetMessage = CreateMessage(newRelease);

            var tweet = Auth.ExecuteOperationWithCredentials(creds, () =>
            {
                return Tweet.PublishTweet(tweetMessage);
            });
        }

        private static string CreateMessage(RepositoryRelease release)
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

            return firstLine +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"See {release.HtmlUrl} for more information." +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"{release.HashTags}";
        }
    }
}
