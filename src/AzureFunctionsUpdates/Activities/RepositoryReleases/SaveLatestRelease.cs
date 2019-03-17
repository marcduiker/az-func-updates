using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Storage;
using AzureFunctionsUpdates.Models.Releases;

namespace AzureFunctionsUpdates.Activities.Releases
{
    public class SaveLatestRelease
    {
        [FunctionName(nameof(SaveLatestRelease))]
        [StorageAccount(Configuration.ConnectionName)]
        public async Task Run(
            [ActivityTrigger] RepositoryRelease repoRelease,
            [Table(Configuration.Releases.TableName)]IAsyncCollector<RepositoryRelease> collector,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(SaveLatestRelease)} for { repoRelease.RepositoryName} { repoRelease.ReleaseName}.");

            await collector.AddAsync(repoRelease);   
        }
    }
}
