using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities
{
    public static class PostUpdate
    {
        [FunctionName(nameof(PostUpdate))]
        public static async Task Run(
            [ActivityTrigger] RepoRelease release,
            ILogger logger)
        {

        }
    }
}
