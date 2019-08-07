using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;

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
