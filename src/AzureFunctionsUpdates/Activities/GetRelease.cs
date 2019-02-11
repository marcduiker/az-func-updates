using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities
{
    public static class GetRelease
    {
        [FunctionName(nameof(GetRelease))]
        public static async Task<RepoRelease> Run(
            [ActivityTrigger] RepoConfiguration repoConfiguration,
            ILogger logger)
        {

        }
    }
}
