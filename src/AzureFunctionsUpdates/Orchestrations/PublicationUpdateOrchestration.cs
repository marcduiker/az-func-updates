﻿using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Activities.Publications;
using AzureFunctionsUpdates.Builders;
using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Models.Publications;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Orchestrations
{
    public class PublicationUpdateOrchestration
    {
        [FunctionName(nameof(PublicationUpdateOrchestration))]
        public async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContextBase context,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(PublicationUpdateOrchestration)}.");

            // Read repo links from storage table
            var publicationConfigurations = await context.CallActivityWithRetryAsync<IReadOnlyList<PublicationConfiguration>>(
                functionName: nameof(GetPublicationConfigurations),
                retryOptions: GetDefaultRetryOptions(),
                input: null);

            if (publicationConfigurations.Any())
            {
                var getLatestPublicationsFromWebTasks = new List<Task<Publication>>();
                var getLatestPublicationsFromHistoryTasks = new List<Task<Publication>>();

                // Fan out over the repos
                foreach (var publicationConfiguration in publicationConfigurations)
                {
                    // Get most recent publications from web/RSS
                    getLatestPublicationsFromWebTasks.Add(context.CallActivityWithRetryAsync<Publication>(
                        nameof(GetLatestPublicationFromWeb),
                        GetDefaultRetryOptions(),
                        publicationConfiguration));

                    // Get most recent known publications from history
                    getLatestPublicationsFromHistoryTasks.Add(context.CallActivityWithRetryAsync<Publication>(
                    nameof(GetLatestPublicationFromHistory),
                    GetDefaultRetryOptions(),
                    publicationConfiguration));   
                }

                var latestFromWeb = await Task.WhenAll(getLatestPublicationsFromWebTasks);
                var latestFromHistory = await Task.WhenAll(getLatestPublicationsFromHistoryTasks);

                var saveAndUpdateTasks = new List<Task>();
                foreach (var publicationConfiguration in publicationConfigurations)
                {
                    var latestPublications = new LatestPublications(publicationConfiguration, latestFromWeb, latestFromHistory);
                    if (latestPublications.IsNewAndShouldBeStored)
                    {
                        saveAndUpdateTasks.Add(context.CallActivityWithRetryAsync(
                            nameof(SaveLatestPublication),
                            GetDefaultRetryOptions(),
                            latestPublications.FromWeb));

                        if (Toggles.DoPostUpdate && latestPublications.IsNewAndShouldBePosted)
                        {
                            var message = MessageBuilder.BuildForPublication(latestPublications.FromWeb);
                            saveAndUpdateTasks.Add(context.CallActivityWithRetryAsync(
                                  nameof(PostUpdate),
                                  GetDefaultRetryOptions(),
                                  message));
                        }
                    }
                }

                await Task.WhenAll(saveAndUpdateTasks);
                logger.LogInformation($"Completed {nameof(PublicationUpdateOrchestration)}.");
            }
        }

        private static RetryOptions GetDefaultRetryOptions()
        {
            return new RetryOptions(TimeSpan.FromMinutes(1), 3);
        }
    }
}
