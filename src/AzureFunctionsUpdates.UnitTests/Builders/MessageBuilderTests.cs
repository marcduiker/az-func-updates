using AzureFunctionsUpdates.Builders;
using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using FluentAssertions;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Builders
{
    public class MessageBuilderTests
    {
        [Fact]
        public void GivenMessageContentIsTooLong_WhenUpdateMessageIsCreated_ThenTheMessageIsShortened()
        {
            // Arrange
            var release = RepositoryReleaseBuilder.BuildOneWithLongRepoAndReleaseName();

            // Act
            var message = MessageBuilder.BuildForRelease(release);

            // Assert
            var messageLengthWithoutUrlPlusFixedShortenedUrl =
                message.Content.Length - release.HtmlUrl.Length + MessageBuilder.TwitterShortenedUrlCharacterCount;
            messageLengthWithoutUrlPlusFixedShortenedUrl.Should().BeLessOrEqualTo(MessageBuilder.MaxTwitterCharacterCount);
        }
        
        [Fact]
        public void GivenMessageContentIsNotTooLong_WhenUpdateMessageIsCreated_ThenTheMessageIsNotShortened()
        {
            // Arrange
            var release = RepositoryReleaseBuilder.BuildOneWithShortRepoAndReleaseNameOnAspecificDate();

            // Act
            var message = MessageBuilder.BuildForRelease(release);

            // Assert
            var messageLengthWithoutUrlPlusFixedShortenedUrl =
                message.Content.Length - release.HtmlUrl.Length + MessageBuilder.TwitterShortenedUrlCharacterCount;
            messageLengthWithoutUrlPlusFixedShortenedUrl.Should().BeLessOrEqualTo(MessageBuilder.MaxTwitterCharacterCount);
        }
        
        [Fact]
        public void GivenARepositoryRelease_WhenUpdateMessageIsCreated_ThenTheMessageContainsTheCorrespondingReleaseInfo()
        {
            // Arrange
            var release = RepositoryReleaseBuilder.BuildOneWithShortRepoAndReleaseNameOnAspecificDate();

            // Act
            var message = MessageBuilder.BuildForRelease(release);

            // Assert
            message.Content.Should().Be(@"A new azure-functions-host release, Azure Functions Runtime 2.0.12477 (tagged v2.0.12477), is available on GitHub since Monday, January 1, 2018.

See https://github.com/Azure/azure-functions-host/releases/tag/v2.0.12477 for more information.

#AzureFunctions #Serverless");
        }
    }
}