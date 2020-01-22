using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.Activities
{
    public class PostUpdateToDeadLetterQueue
    {
        [return: Queue("azfuncupdates-postupdate-deadletter", Connection = "TableStorageConnection")]
        [FunctionName(nameof(PostUpdateToDeadLetterQueue))]
        public UpdateMessage Run(
          [ActivityTrigger] UpdateMessage input)
        {
            return input;
        }
    }
}
