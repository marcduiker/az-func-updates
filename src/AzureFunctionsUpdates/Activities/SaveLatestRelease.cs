using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using AzureFunctionsUpdates.Storage;

namespace AzureFunctionsUpdates.Activities
{
    public static class SaveLatestRelease
    {
        [FunctionName(nameof(SaveLatestRelease))]
        [StorageAccount(Configuration.ConnectionName)]
        public static async Task Run(
            [ActivityTrigger] RepositoryRelease repoRelease,
            [Table(Configuration.Releases.TableName)]ICollector<RepositoryRelease> collector,
            ILogger logger)
        {
            try
            {
                collector.Add(repoRelease);
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to store {repoRelease.RepositoryName} release {repoRelease.ReleaseName}.", e);
            }
        }
    }
}
