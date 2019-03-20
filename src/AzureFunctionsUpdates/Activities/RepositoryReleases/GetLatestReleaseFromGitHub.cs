using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Octokit;
using AzureFunctionsUpdates.Models.Releases;

namespace AzureFunctionsUpdates.Activities.Releases
{
    public class GetLatestReleaseFromGitHub
    {
        private readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("AzureFunctionUpdates2019"));

        [FunctionName(nameof(GetLatestReleaseFromGitHub))]
        public async Task<RepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromGitHub)} for { repoConfiguration.RepositoryOwner } { repoConfiguration.RepositoryName }.");

            RepositoryRelease repoRelease = new NullRelease(repoConfiguration.RepositoryName);
            try
            {
                var latestRelease = await client.Repository.Release.GetLatest(repoConfiguration.RepositoryOwner, repoConfiguration.RepositoryName);
                repoRelease = MapToRepoRelease(repoConfiguration, latestRelease);
            }
            catch (NotFoundException)
            {
                // We're ignoring 404s for Releases since there are repositories without releases.
                // But we want to keep monitoring them and notify once they do have a releases.
                logger.LogWarning($"No release information found for repository: {repoConfiguration.RepositoryName}.");
            }

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
                release.Body,
                repoConfiguration.HashTags);
        }
    }
}
