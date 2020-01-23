using System;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using AzureFunctionsUpdates.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class SaveLatestRelease
    {
        [FunctionName(nameof(SaveLatestRelease))]
        [return: Table(Configuration.Releases.TableName, Connection = Configuration.ConnectionName)]
        public RepositoryRelease Run(
            [ActivityTrigger] RepositoryRelease repoRelease,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestRelease)} for " +
                $"{ repoRelease.RepositoryName} " +
                $"{ repoRelease.ReleaseName}.");
            
            return repoRelease;  
        }
    }
}
