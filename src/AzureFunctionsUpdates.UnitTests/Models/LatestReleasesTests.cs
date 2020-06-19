using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using FluentAssertions;
using System;
using AzureFunctionsUpdates.Builders;
using AzureFunctionsUpdates.Models.RepositoryReleases;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Models
{
    public class LatestReleasesTests
    {
        [Fact]
        public void GivenHistoryReleaseIsNullRelease_WhenIsNewAndShouldBeStoredIsCalled_ThenResultShouldBeTrue()
        {
            // Arrange
            const string repoName = "repo";
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId<GitHubRepositoryRelease>(repoName, 1);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneNullRelease<HistoryRepositoryRelease>(repoName);
            var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();
            
            // Act
            var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                repoConfig, 
                releasesFromGitHub, 
                releasesFromHistory,
                releaseMatchFunction);

            // Assert
            latestReleases.IsNewAndShouldBeStored.Should().BeTrue("because no release was found in history data.");
        }

        [Fact]
        public void GivenHistoryReleaseIsReleaseWithMatchingReleaseId_WhenIsNewAndShouldBeStoredIsCalled_ThenResultShouldBeFalse()
        {
            // Arrange
            const string repoName = "repo";
            const int releaseId = 1;
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId<GitHubRepositoryRelease>(repoName, releaseId);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId<HistoryRepositoryRelease>(repoName, releaseId);
            var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();
            
            // Act
            var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                repoConfig, 
                releasesFromGitHub, 
                releasesFromHistory,
                releaseMatchFunction);

            // Assert
            latestReleases.IsNewAndShouldBeStored.Should().BeFalse("because the releaseIds are equal");
        }

        [Fact]
        public void GivenHistoryReleaseIsNullReleaseAndGitHubReleaseIsNullRelease_WhenIsNewAndShouldBeStoredIsCalled_ThenResultShouldBeFalse()
        {
            // Arrange
            const string repoName = "repo";
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneNullRelease<GitHubRepositoryRelease>(repoName);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneNullRelease<HistoryRepositoryRelease>(repoName);
            var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();
            
            // Act
            var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                repoConfig,
                releasesFromGitHub, 
                releasesFromHistory,
                releaseMatchFunction);

            // Assert
            latestReleases.IsNewAndShouldBeStored.Should().BeFalse("because there is no result from GitHub");
        }

        [Fact]
        public void GivenHistoryReleaseIsReleaseWithNonMatchingReleaseId_WhenIsNewAndShouldBeStoredIsCalled_ThenResultShouldBeTrue()
        {
            // Arrange
            const string repoName = "repo";
            const int releaseIdHistory = 1;
            const int releaseIdGithub = 2;
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId<GitHubRepositoryRelease>(repoName, releaseIdGithub);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId<HistoryRepositoryRelease>(repoName, releaseIdHistory);
            var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();

            // Act
            var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                repoConfig, 
                releasesFromGitHub, 
                releasesFromHistory,
                releaseMatchFunction);

            // Assert
            latestReleases.IsNewAndShouldBeStored.Should().BeTrue("because the releaseIds are not equal");
        }

        [Fact]
        public void GivenHistoryReleaseIsNullReleaseAndGitHubReleaseIsWithinTimeWindow_WhenIsNewAndShouldBePostedIsCalled_ThenResultShouldBeTrue()
        {
            // Arrange
            const string repoName = "repo";
            const int releaseIdGithub = 1;
            var daysTimespan = new TimeSpan(1, 0, 0, 0);
            var gitHubReleaseDate = DateTimeOffset.UtcNow.Subtract(daysTimespan);
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseIdAndDate<GitHubRepositoryRelease>(repoName, releaseIdGithub, gitHubReleaseDate);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneNullRelease<HistoryRepositoryRelease>(repoName);
            var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();

            // Act
            var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                repoConfig, 
                releasesFromGitHub,
                releasesFromHistory,
                releaseMatchFunction);

            // Assert
            latestReleases.IsNewAndShouldBeStored.Should().BeTrue("because the release is not in history yet.");
            latestReleases.IsNewAndShouldBePosted.Should().BeTrue($"because the release date is within the time window of {LatestReleases.MaximumNumberOfDaysToPostAboutNewlyFoundRelease} days");
        }

        [Fact]
        public void GivenHistoryReleaseIsNullReleaseAndGitHubReleaseIsOutsideTimeWindow_WhenIsNewAndShouldBePostedIsCalled_ThenResultShouldBeFalse()
        {
            // Arrange
            const string repoName = "repo";
            const int releaseIdGithub = 1;
            var daysTimespan = new TimeSpan(8, 0, 0, 0);
            var gitHubReleaseDate = DateTimeOffset.UtcNow.Subtract(daysTimespan);
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseIdAndDate<GitHubRepositoryRelease>(repoName, releaseIdGithub, gitHubReleaseDate);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneNullRelease<HistoryRepositoryRelease>(repoName);
            var releaseMatchFunction = ReleaseFunctionBuilder.BuildForMatchingRepositoryName();

            // Act
            var latestReleases = LatestObjectsBuilder.Build<RepositoryConfiguration, RepositoryRelease, LatestReleases>(
                repoConfig, 
                releasesFromGitHub, 
                releasesFromHistory,
                releaseMatchFunction);

            // Assert
            latestReleases.IsNewAndShouldBeStored.Should().BeTrue("because the release is not in history yet.");
            latestReleases.IsNewAndShouldBePosted.Should().BeFalse($"because the release date is outside the time window of {LatestReleases.MaximumNumberOfDaysToPostAboutNewlyFoundRelease} days");
        }
    }
}
