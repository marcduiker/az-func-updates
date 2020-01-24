using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using UpdateMessage = AzureFunctionsUpdates.Models.UpdateMessage;

namespace AzureFunctionsUpdates.Activities
{
    public class PostUpdateActivity
    {
        private readonly string consumerApiKey = Environment.GetEnvironmentVariable("Twitter_Consumer_Api_Key");
        private readonly string consumerApiSecret = Environment.GetEnvironmentVariable("Twitter_Consumer_Api_Secret");
        private readonly string accessToken = Environment.GetEnvironmentVariable("Twitter_Access_Token");
        private readonly string accessTokenSecret = Environment.GetEnvironmentVariable("Twitter_Access_Token_Secret");

        [FunctionName(nameof(PostUpdateActivity))]
        public Task<bool> Run(
            [ActivityTrigger] UpdateMessage message,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(PostUpdateActivity)} for { message.Topic}.");

            var creds = new TwitterCredentials(consumerApiKey, consumerApiSecret, accessToken, accessTokenSecret);
            
            var tweet = Auth.ExecuteOperationWithCredentials(creds, () => Tweet.PublishTweet(message.Content));
            
            logger.LogInformation($"Finished {nameof(PostUpdateActivity)} with tweet: {tweet.Url}.");

            return Task.FromResult(tweet.IsTweetPublished);
        }
    }
}
