using AzureFunctionsUpdates.Orchestrations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Clients
{
    public class TimerBasedStarter
    {
        [FunctionName(nameof(TimerBasedStarter))]
        public async Task Run([TimerTrigger("0 0 */2 * * *")]TimerInfo timer,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger logger)
        {
            await client.StartNewAsync(nameof(ReleaseUpdateOrchestration), null);
        }
    }
}
