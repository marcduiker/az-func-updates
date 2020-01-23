using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Activities;
using Microsoft.Azure.WebJobs;
using Moq;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Activities.RepositoryReleases;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class ReleaseUpdateOrchestrationContextBuilder
    {
        public static Mock<IDurableOrchestrationContext> BuildWithoutHistoryAndWithGitHubRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            var mockContext = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetRepositoryConfigurations),
                        It.IsAny<RetryOptions>(),
                        null))
                .ReturnsAsync(repoConfigurations);

            // Setup GetLatestReleaseFromGitHub
            var gitHubRepositoryReleaseRepo1 = RepositoryReleaseBuilder.BuildOne<GitHubRepositoryRelease>(repository1Name);
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                 .ReturnsAsync(gitHubRepositoryReleaseRepo1);

            var gitHubRepositoryReleaseRepo2 = RepositoryReleaseBuilder.BuildOne<GitHubRepositoryRelease>(repository2Name);
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(gitHubRepositoryReleaseRepo2);

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease<HistoryNullRelease>(repository1Name));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease<HistoryNullRelease>(repository2Name));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(gitHubRepositoryReleaseRepo1);

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository2Name))))
                .ReturnsAsync(gitHubRepositoryReleaseRepo2);

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository1Name))))
                .ReturnsAsync(true);

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository2Name))))
                .ReturnsAsync(true);

            return mockContext;
        }

        public static Mock<IDurableOrchestrationContext> BuildWithoutHistoryAndGitHubReturnsNullRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            var mockContext = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
            var repoConfigurations = RepositoryConfigurationBuilder.BuildTwo(repository1Name, repository2Name);

            // Setup GetRepositoryConfigurations
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<RepositoryConfiguration>>(
                        nameof(GetRepositoryConfigurations),
                        It.IsAny<RetryOptions>(),
                        null))
                .ReturnsAsync(repoConfigurations);

            // Setup GetLatestReleaseFromGitHub
            var gitHubReleaseRepo1 = RepositoryReleaseBuilder.BuildOne<GitHubRepositoryRelease>(repository1Name);
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                 .ReturnsAsync(gitHubReleaseRepo1);


            // Returns NullRelease because no release info is retrieved from GitHub
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease<GitHubNullRelease>(repository2Name));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease<HistoryNullRelease>(repository1Name));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease<HistoryNullRelease>(repository2Name));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(gitHubReleaseRepo1);

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository1Name))))
                .ReturnsAsync(true);

            return mockContext;
        }

        public static Mock<IDurableOrchestrationContext> BuildWithHistoryAndWithGitHubWithEqualReleases()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            const int releaseIdRepo1 = 2;
            const int releaseIdRepo2 = 5;
            var mockContext = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
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
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<GitHubRepositoryRelease>(repository1Name, releaseIdRepo1));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<GitHubRepositoryRelease>(repository2Name, releaseIdRepo2));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<HistoryRepositoryRelease>(repository1Name, releaseIdRepo1));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<HistoryRepositoryRelease>(repository2Name, releaseIdRepo2));

            return mockContext;
        }

        public static Mock<IDurableOrchestrationContext> BuildWithHistoryAndWithGitHubWithOneEqualAndOneDifferentRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            const int releaseIdHistoryRepo1 = 2;
            const int releaseIdHistoryRepo2 = 5;
            const int releaseIdGithubRepo1 = releaseIdHistoryRepo1;
            const int releaseIdGithubRepo2 = 4; // note that this is lower than previous Id but we're only checking on equality
            var mockContext = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
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
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<GitHubRepositoryRelease>(repository1Name, releaseIdGithubRepo1));

            var gitHubRepositoryReleaseRepo2 = RepositoryReleaseBuilder.BuildOneWithReleaseId<GitHubRepositoryRelease>(
                    repository2Name,
                    releaseIdGithubRepo2);
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(gitHubRepositoryReleaseRepo2);

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<HistoryRepositoryRelease>(repository1Name, releaseIdHistoryRepo1));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<HistoryRepositoryRelease>(repository2Name, releaseIdHistoryRepo2));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository2Name))))
                .ReturnsAsync(gitHubRepositoryReleaseRepo2);

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository2Name))))
                .ReturnsAsync(true);

            return mockContext;
        }
        
         public static Mock<IDurableOrchestrationContext> BuildWithHistoryAndWithGitHubWithDifferentReleasesButFailsOnSaveLatestRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const int releaseIdHistoryRepo1 = 2;
            const int releaseIdGithubRepo1 = 3;
            var mockContext = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
            var repoConfigurations = RepositoryConfigurationBuilder.BuildListWithOne(repository1Name);

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
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<GitHubRepositoryRelease>(repository1Name, releaseIdGithubRepo1));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId<HistoryRepositoryRelease>(repository1Name, releaseIdHistoryRepo1));

           
            // Setup SaveLatestRelease, throws
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository1Name))))
                .Returns(Task.FromException<RepositoryRelease>(new FunctionFailedException("failed")));

            // PostUpdate should not be called

            return mockContext;
        }
    }
}
