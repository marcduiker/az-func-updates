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
        public void GivenNoReleasesAreAvailableInHistoryAndNewGithubReleasesAreRetrieved_WhenOrchestrationIsRun_ThenNewUpdatesShouldBePosted()
        {
            // Arrange
            const string repo1Name = "repo-1";
            const string repo2Name = "repo-2";
            var mockContext = OrchestrationContextBuilder.BuildWithoutHistoryAndWithGitHubRelease(repo1Name, repo2Name);
            var logger = new Mock<ILogger>();

            // Act
            ReleaseUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.Verify(c => c.CallActivityWithRetryAsync(
                        nameof(SaveLatestRelease),
                        It.IsAny<RetryOptions>(),
                        It.IsAny<RepositoryRelease>()), Times.Exactly(2));
        }
    }
}
