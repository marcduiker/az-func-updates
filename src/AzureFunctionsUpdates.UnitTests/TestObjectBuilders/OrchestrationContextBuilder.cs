using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Activities;
using Microsoft.Azure.WebJobs;
using Moq;
using System.Collections.Generic;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class OrchestrationContextBuilder
    {
        public static Mock<DurableOrchestrationContextBase> BuildWithoutHistoryAndWithGitHubRelease(
            string repository1Name,
            string repository2Name)
        {
            var mockContext = new Mock<DurableOrchestrationContextBase>();
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetRepositoryConfigurations),
                        It.IsAny<RetryOptions>(),
                        null))
                .ReturnsAsync(repoConfigurations);

            // Setup GetLatestReleaseFromGitHub
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[0]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOne(repository1Name));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[1]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOne(repository2Name));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[0]))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository1Name));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       repoConfigurations[1]))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository2Name));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()));

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()));

            return mockContext;
        }
    }
}
