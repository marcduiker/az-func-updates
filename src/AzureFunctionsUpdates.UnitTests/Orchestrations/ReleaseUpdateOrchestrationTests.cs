using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Orchestrations;
using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using Microsoft.Extensions.Logging;
using Moq;
using System;
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
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");
            var mockContext = ReleaseUpdateOrchestrationContextBuilder.BuildWithoutHistoryAndWithGitHubRelease();
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
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");
            var mockContext = ReleaseUpdateOrchestrationContextBuilder.BuildWithoutHistoryAndGitHubReturnsNullRelease();
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
            var mockContext = ReleaseUpdateOrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithEqualReleases();
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
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");
            var mockContext = ReleaseUpdateOrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithOneEqualAndOneDifferentRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            await releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }
        
        [Fact]
        public async Task GivenReleasesAreAvailableInHistoryAndOneGithubReleasesDifferent_WhenOrchestrationIsRunAndSaveLatestReleaseFails_ThenPostUpdateShouldNotBeCalled()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");
            var mockContext = ReleaseUpdateOrchestrationContextBuilder.BuildWithHistoryAndWithGitHubWithDifferentReleasesButFailsOnSaveLatestRelease();
            var logger = new Mock<ILogger>();
            var releaseUpdateOrchestration = new ReleaseUpdateOrchestration();

            // Act
            await releaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }
    }
}
