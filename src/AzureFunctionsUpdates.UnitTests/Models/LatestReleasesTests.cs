using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Models
{
    public class LatestReleasesTests
    {
        [Fact]
        public void GivenHistoryReleaseIsNullRelease_WhenIsNewReleaseIsCalled_ThenIsNewReleaseShouldBeTrue()
        {
            // Arrange
            const string repoName = "repo";
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId(repoName, 1);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneNullRelease(repoName);

            // Act
            var latestReleases = new LatestReleases(repoConfig, releasesFromGitHub, releasesFromHistory);

            // Assert
            latestReleases.IsNewRelease.Should().BeTrue("because no release was found in history data.");
        }

        [Fact]
        public void GivenHistoryReleaseIsReleaseWithMatchingReleaseId_WhenIsNewReleaseIsCalled_ThenIsNewReleaseShouldBeFalse()
        {
            // Arrange
            const string repoName = "repo";
            const int releaseId = 1;
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId(repoName, releaseId);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId(repoName, releaseId);

            // Act
            var latestReleases = new LatestReleases(repoConfig, releasesFromGitHub, releasesFromHistory);

            // Assert
            latestReleases.IsNewRelease.Should().BeFalse("because the releaseIds are equal");
        }

        [Fact]
        public void GivenHistoryReleaseIsReleaseWithNonMatchingReleaseId_WhenIsNewReleaseIsCalled_ThenIsNewReleaseShouldBeTrue()
        {
            // Arrange
            const string repoName = "repo";
            const int releaseIdHistory = 1;
            const int releaseIdGithub = 2;
            var repoConfig = RepositoryConfigurationBuilder.BuildOne(repoName);
            var releasesFromGitHub = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId(repoName, releaseIdGithub);
            var releasesFromHistory = RepositoryReleaseBuilder.BuildListContainingOneWithReleaseId(repoName, releaseIdHistory);

            // Act
            var latestReleases = new LatestReleases(repoConfig, releasesFromGitHub, releasesFromHistory);

            // Assert
            latestReleases.IsNewRelease.Should().BeTrue("because the releaseIds are not equal");
        }
    }
}
