using AzureFunctionsUpdates.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Octokit;

namespace AzureFunctionsUpdates.Activities
{
    public static class GetLatestReleaseFromGitHub
    {
        private static readonly GitHubClient Client = new GitHubClient(new ProductHeaderValue("AzureFunctionUpdates2019"));

        [FunctionName(nameof(GetLatestReleaseFromGitHub))]
        public static async Task<RepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            ILogger logger)
        {
            var latestRelease = await Client.Repository.Release.GetLatest(repoConfiguration.RepositoryOwner, repoConfiguration.RepositoryName);
            var repoRelease = MapToRepoRelease(repoConfiguration, latestRelease);

            return repoRelease;
        }

        private static RepositoryRelease MapToRepoRelease(
            RepositoryConfiguration repoConfiguration, 
            Release release)
        {
            return new RepositoryRelease(
                repoConfiguration.RepositoryName,
                release.Id,
                release.Name,
                release.TagName,
                release.CreatedAt,
                release.HtmlUrl,
                release.Body);
        }
    }
}
