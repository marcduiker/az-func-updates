using AzureFunctionsUpdates.Orchestrations;
using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Orchestrations
{
    public class ReleaseUpdateOrchestrationTests
    {
        [Fact]
        public async Task GivenNoReleasesAreAvailableInHistoryAndNewGithubReleasesAreRetrieved_WhenOrchestrationIsRunForTwoRepos_ThenSaveAndPostShouldBeCalled()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithoutHistoryAndWithGitHubRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            await releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }

        [Fact]
        public async Task GivenNoReleasesAreAvailableInHistoryAndNewGithubReleaseReturnsNullRelease_WhenOrchestrationIsRunForTwoRepos_ThenSaveAndPostShouldBeCalledForTheReleaseWhichWasReturnedFromGitHub()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithoutHistoryAndGitHubReturnsNullRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            await releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }


        [Fact]
        public async Task GivenReleasesAreAvailableInHistoryAndNewGithubReleasesAreTheSame_WhenOrchestrationIsRun_ThenSaveAndPostShouldNotBeCalled()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithEqualReleases();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            await releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }

        [Fact]
        public async Task GivenReleasesAreAvailableInHistoryAndOneGithubReleaseIsEqualAndOneIsDifferent_WhenOrchestrationIsRun_ThenSaveAndPostShouldBeCalledOnce()
        {
            // Arrange
            var mockContext = OrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithOneEqualAndOneDifferentRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            await releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }
    }
}
