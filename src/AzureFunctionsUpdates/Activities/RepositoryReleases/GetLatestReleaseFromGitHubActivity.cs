using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Octokit;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class GetLatestReleaseFromGitHubActivity
    {
        private readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("AzureFunctionUpdates2019"));

        [FunctionName(nameof(GetLatestReleaseFromGitHubActivity))]
        public async Task<RepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromGitHubActivity)} for " +
                $"{ repoConfiguration.RepositoryOwner } " +
                $"{ repoConfiguration.RepositoryName }.");

            RepositoryRelease repoRelease = new GitHubNullRelease(repoConfiguration.RepositoryName);
            try
            {
                var releases = await client.Repository.Release.GetAll(
                    repoConfiguration.RepositoryOwner,
                    repoConfiguration.RepositoryName);

                // A repository might not use releases yet.
                if (releases.Any())
                {
                    var latestRelease = releases.OrderByDescending(r => r.PublishedAt ?? r.CreatedAt).FirstOrDefault();
                    if (latestRelease != null)
                    {
                        repoRelease = MapToRepoRelease(repoConfiguration, latestRelease);
                    }
                }
            }
            catch (NotFoundException)
            {
                // We're ignoring 404s for Releases since there are repositories without releases.
                // But we want to keep monitoring them and notify once they do have a releases.
                logger.LogWarning("No release information found for repository={repositoryName}", repoConfiguration.RepositoryName);
            }

            return repoRelease;
        }

        private static GitHubRepositoryRelease MapToRepoRelease(
            RepositoryConfiguration repoConfiguration, 
            Release release)
        {
            return new GitHubRepositoryRelease(
                repoConfiguration.RepositoryName,
                release.Id,
                release.Name,
                release.TagName,
                release.PublishedAt ?? release.CreatedAt,
                release.HtmlUrl,
                release.Body,
                repoConfiguration.HashTags);
        }
    }
}
