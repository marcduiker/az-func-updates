using System;
using AzureFunctionsUpdates.Application;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureFunctionsUpdates.Application
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var gitHubClient = new GitHubClient(
                new ProductHeaderValue("AzureFunctionUpdates2020"))
            {
                Credentials = new Credentials(Environment.GetEnvironmentVariable("Octokit_PAT")) 
            };
            
            builder.Services.AddSingleton<IGitHubClient>(gitHubClient);
        }
    }
}