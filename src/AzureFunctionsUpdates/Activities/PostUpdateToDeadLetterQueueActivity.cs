using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.Activities
{
    public class PostUpdateToDeadLetterQueueActivity
    {
        [FunctionName(nameof(PostUpdateToDeadLetterQueueActivity))]
        [return: Queue("azfuncupdates-postupdate-deadletter", Connection = "TableStorageConnection")]
        public UpdateMessage Run(
          [ActivityTrigger] UpdateMessage input)
        {
            return input;
        }
    }
}
