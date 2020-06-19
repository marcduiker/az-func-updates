using System;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.Builders
{
    public static class RetryOptionsBuilder
    {
        public static RetryOptions BuildDefault()
        {
            return new RetryOptions(TimeSpan.FromMinutes(1), 3);
        }
    }
}