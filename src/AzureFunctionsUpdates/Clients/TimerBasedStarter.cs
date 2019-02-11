using AzureFunctionsUpdates.Orchestrations;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Clients
{
    public static class TimerBasedStarter
    {
        [FunctionName(nameof(TimerBasedStarter))]
        public static async Task Run([TimerTrigger("0 0 */1 * * *")]TimerInfo timer,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger logger)
        {
            await client.StartNewAsync(nameof(RepoUpdateOrchestration), null);
        }
    }
}
