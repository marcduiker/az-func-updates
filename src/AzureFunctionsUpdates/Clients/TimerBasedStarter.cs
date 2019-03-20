using AzureFunctionsUpdates.Orchestrations;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Clients
{
    public class TimerBasedStarter
    {
        [FunctionName(nameof(TimerBasedStarter))]
        public async Task Run([TimerTrigger("0 0 */1 * * *")]TimerInfo timer,
            [OrchestrationClient] DurableOrchestrationClient client,
            ILogger logger)
        {
            await client.StartNewAsync(nameof(ReleaseUpdateOrchestration), null);
            await client.StartNewAsync(nameof(PublicationUpdateOrchestration), null);
        }
    }
}
