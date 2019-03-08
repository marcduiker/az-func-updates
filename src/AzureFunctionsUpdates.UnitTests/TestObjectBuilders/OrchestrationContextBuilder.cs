using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Activities;
using Microsoft.Azure.WebJobs;
using Moq;
using System.Collections.Generic;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class OrchestrationContextBuilder
    {
        public static Mock<DurableOrchestrationContextBase> BuildWithoutHistoryAndWithGitHubRelease()
        {
            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            var mockContext = new Mock<DurableOrchestrationContextBase>();
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetConfigurations),
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

        public static Mock<DurableOrchestrationContextBase> BuildWithoutHistoryAndGitHubReturnsNullRelease()
        {
            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            var mockContext = new Mock<DurableOrchestrationContextBase>();
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetConfigurations),
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


            // Returns NullRelease because no release info is retrieved from GitHub
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[1]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository1Name));

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

        public static Mock<DurableOrchestrationContextBase> BuildWithHistoryAndWithGitHubWithEqualReleases()
        {
            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            const int releaseIdRepo1 = 2;
            const int releaseIdRepo2 = 5;
            var mockContext = new Mock<DurableOrchestrationContextBase>();
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetConfigurations),
                        It.IsAny<RetryOptions>(),
                        null))
                .ReturnsAsync(repoConfigurations);

            // Setup GetLatestReleaseFromGitHub
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[0]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdRepo1));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[1]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdRepo2));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[0]))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdRepo1));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       repoConfigurations[1]))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdRepo2));

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

        public static Mock<DurableOrchestrationContextBase> BuildWithHistoryAndWithGitHubWithOneEqualAndOneDifferentRelease()
        {
            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            const int releaseIdHistoryRepo1 = 2;
            const int releaseIdHistoryRepo2 = 5;
            const int releaseIdGithubRepo1 = releaseIdHistoryRepo1;
            const int releaseIdGithubRepo2 = 4; // note that this is lower than previous Id but we're only checking on equality
            var mockContext = new Mock<DurableOrchestrationContextBase>();
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetConfigurations),
                        It.IsAny<RetryOptions>(),
                        null))
                .ReturnsAsync(repoConfigurations);

            // Setup GetLatestReleaseFromGitHub
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[0]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdGithubRepo1));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[1]))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdGithubRepo2));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        repoConfigurations[0]))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdHistoryRepo1));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       repoConfigurations[1]))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdHistoryRepo2));

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
