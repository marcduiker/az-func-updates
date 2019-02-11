using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Activities
{
    public static class GetRepositories
    {
        [FunctionName(nameof(GetRepositories))]
        public static async Task<IReadOnlyCollection<RepoConfiguration>> Run(
            [ActivityTrigger] string input,
            ILogger logger)
        {

        }
    }
}
