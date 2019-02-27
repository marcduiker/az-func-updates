using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using Tweetinvi;
using Tweetinvi.Models;

namespace AzureFunctionsUpdates.Activities
{
    public static class PostUpdate
    {
        private static string consumerApiKey = Environment.GetEnvironmentVariable("Twitter_Consumer_Api_Key");
        private static string consumerApiSecret = Environment.GetEnvironmentVariable("Twitter_Consumer_Api_Secret");
        private static string accessToken = Environment.GetEnvironmentVariable("Twitter_Access_Token");
        private static string accessTokenSecret = Environment.GetEnvironmentVariable("Twitter_Access_Token_Secret");

        [FunctionName(nameof(PostUpdate))]
        public static void Run(
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
             return $"A new release ({release.ReleaseName}) for {release.RepositoryName} is available on GitHub since {release.ReleaseCreatedAt.ToString("D")}." +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"See {release.HtmlUrl} for more information." +
               $"{Environment.NewLine}" +
               $"{Environment.NewLine}" +
               $"{release.HashTags }";
        }
    }
}
