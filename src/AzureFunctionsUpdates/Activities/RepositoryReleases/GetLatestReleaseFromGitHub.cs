﻿using System.Linq;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Octokit;

namespace AzureFunctionsUpdates.Activities.RepositoryReleases
{
    public class GetLatestReleaseFromGitHub
    {
        private readonly GitHubClient client = new GitHubClient(new ProductHeaderValue("AzureFunctionUpdates2019"));

        [FunctionName(nameof(GetLatestReleaseFromGitHub))]
        public async Task<RepositoryRelease> Run(
            [ActivityTrigger] RepositoryConfiguration repoConfiguration,
            ILogger logger)
        {
            logger.LogInformation($"Started {nameof(GetLatestReleaseFromGitHub)} for " +
                $"{ repoConfiguration.RepositoryOwner } " +
                $"{ repoConfiguration.RepositoryName }.");

            RepositoryRelease repoRelease = new NullRelease(repoConfiguration.RepositoryName);
            try
            {
                var latestRelease = await client.Repository.Release.GetAll(
                    repoConfiguration.RepositoryOwner,
                    repoConfiguration.RepositoryName,
                    new ApiOptions { PageCount = 1, PageSize = 1 });
                repoRelease = MapToRepoRelease(repoConfiguration, latestRelease.FirstOrDefault());
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
