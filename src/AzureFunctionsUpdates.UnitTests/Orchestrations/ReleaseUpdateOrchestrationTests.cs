using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Orchestrations;
using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Orchestrations
{
    public class ReleaseUpdateOrchestrationTests
    {
        [Fact]
        public void GivenNoReleasesAreAvailableInHistoryAndNewGithubReleasesAreRetrieved_WhenOrchestrationIsRunForTwoRepos_ThenSaveAndPostShouldBeCalled()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithoutHistoryAndWithGitHubRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()), Times.Exactly(2));

            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                       nameof(PostUpdate),
                       It.IsAny<RetryOptions>(),
                       It.IsAny<RepositoryRelease>()), Times.Exactly(2));
        }

        [Fact]
        public void GivenNoReleasesAreAvailableInHistoryAndNewGithubReleaseReturnsNullRelease_WhenOrchestrationIsRunForTwoRepos_ThenSaveAndPostShouldBeCalledForTheReleaseWhichWasReturnedFromGitHub()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithoutHistoryAndGitHubReturnsNullRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()), Times.Exactly(1));

            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                       nameof(PostUpdate),
                       It.IsAny<RetryOptions>(),
                       It.IsAny<RepositoryRelease>()), Times.Exactly(1));
        }


        [Fact]
        public void GivenReleasesAreAvailableInHistoryAndNewGithubReleasesAreTheSame_WhenOrchestrationIsRun_ThenSaveAndPostShouldNotBeCalled()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithEqualReleases();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()), Times.Never);

            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                       nameof(PostUpdate),
                       It.IsAny<RetryOptions>(),
                       It.IsAny<RepositoryRelease>()), Times.Never);
        }

        [Fact]
        public void GivenReleasesAreAvailableInHistoryAndOneGithubReleaseIsEqualAndOneIsDifferent_WhenOrchestrationIsRun_ThenSaveAndPostShouldBeCalledOnce()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithOneEqualAndOneDifferentRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()), Times.Once);

            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                       nameof(PostUpdate),
                       It.IsAny<RetryOptions>(),
                       It.IsAny<RepositoryRelease>()), Times.Once);
        }
    }
}
