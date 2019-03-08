using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;

namespace AzureFunctionsUpdates.Activities
{
    public class SaveLatestRelease
    {
        [FunctionName(nameof(SaveLatestRelease))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task Run(
            [ActivityTrigger] RepositoryRelease repoRelease,
            [Table(Configuration.Releases.TableName)]ICollector<RepositoryRelease> collector,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestRelease)} for { repoRelease.RepositoryName} { repoRelease.ReleaseName}.");

            collector.Add(repoRelease);   
        }
    }
}
