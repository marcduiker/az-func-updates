using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Activities;
using Microsoft.Azure.WebJobs;
using Moq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using AzureFunctionsUpdates.Activities.RepositoryReleases;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class ReleaseUpdateOrchestrationContextBuilder
    {
        public static Mock<DurableOrchestrationContextBase> BuildWithoutHistoryAndWithGitHubRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            var mockContext = new Mock<DurableOrchestrationContextBase>(MockBehavior.Strict);
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
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOne(repository1Name));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOne(repository2Name));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository1Name));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository2Name));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(true);

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository2Name))))
                .ReturnsAsync(true);

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository1Name))))
                .Returns(Task.CompletedTask);

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository2Name))))
                .Returns(Task.CompletedTask);

            return mockContext;
        }

        public static Mock<DurableOrchestrationContextBase> BuildWithoutHistoryAndGitHubReturnsNullRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            var mockContext = new Mock<DurableOrchestrationContextBase>(MockBehavior.Strict);
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
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOne(repository1Name));


            // Returns NullRelease because no release info is retrieved from GitHub
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository2Name));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository1Name));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildNullRelease(repository2Name));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(true);

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository1Name))))
                .Returns(Task.CompletedTask);

            return mockContext;
        }

        public static Mock<DurableOrchestrationContextBase> BuildWithHistoryAndWithGitHubWithEqualReleases()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            const int releaseIdRepo1 = 2;
            const int releaseIdRepo2 = 5;
            var mockContext = new Mock<DurableOrchestrationContextBase>(MockBehavior.Strict);
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
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdRepo1));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdRepo2));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdRepo1));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdRepo2));

            return mockContext;
        }

        public static Mock<DurableOrchestrationContextBase> BuildWithHistoryAndWithGitHubWithOneEqualAndOneDifferentRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const string repository2Name = "repo-2";
            const int releaseIdHistoryRepo1 = 2;
            const int releaseIdHistoryRepo2 = 5;
            const int releaseIdGithubRepo1 = releaseIdHistoryRepo1;
            const int releaseIdGithubRepo2 = 4; // note that this is lower than previous Id but we're only checking on equality
            var mockContext = new Mock<DurableOrchestrationContextBase>(MockBehavior.Strict);
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
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdGithubRepo1));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromGitHub),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdGithubRepo2));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdHistoryRepo1));

            mockContext
               .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                       nameof(GetLatestReleaseFromHistory),
                       It.IsAny<RetryOptions>(),
                       It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository2Name))))
               .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository2Name, releaseIdHistoryRepo2));

            // Setup SaveLatestRelease
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository2Name))))
                .ReturnsAsync(true);

            // Setup PostUpdate
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                        nameof(PostUpdate),
                        It.IsAny<RetryOptions>(),
                        It.Is<UpdateMessage>(message => message.Topic.Contains(repository2Name))))
                .Returns(Task.CompletedTask);

            return mockContext;
        }
        
         public static Mock<DurableOrchestrationContextBase> BuildWithHistoryAndWithGitHubWithDifferentReleasesButFailsOnSaveLatestRelease()
        {
            // Enable the orchestration to post updates.
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            const string repository1Name = "repo-1";
            const int releaseIdHistoryRepo1 = 2;
            const int releaseIdGithubRepo1 = 3;
            var mockContext = new Mock<DurableOrchestrationContextBase>(MockBehavior.Strict);
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
                 .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdGithubRepo1));

            // Setup GetLatestReleaseFromHistory
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<RepositoryRelease>(
                        nameof(GetLatestReleaseFromHistory),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryConfiguration>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(RepositoryReleaseBuilder.BuildOneWithReleaseId(repository1Name, releaseIdHistoryRepo1));

           
            // Setup SaveLatestRelease, returns false do to exception
            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.Is<RepositoryRelease>(r => r.RepositoryName.Equals(repository1Name))))
                .ReturnsAsync(false);

            // PostUpdate should not be called

            return mockContext;
        }
    }
}
