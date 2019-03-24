using AzureFunctionsUpdates.Storage;
using FluentAssertions;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Storage
{
    public class KeyFormatterTests
    {
        [Theory]
        [InlineData("abcd123", "abcd123")]
        [InlineData("abcd-123", "abcd-123")]
        [InlineData("abcd?123", "abcd-123")]
        [InlineData("abcd#123#", "abcd-123-")]
        [InlineData("abcd/123", "abcd-123")]
        [InlineData("abcd\\1?2#3", "abcd-1-2-3")]
        public void SanitizeKeyTests(string oldKey, string newKey)
        {
            // Act
            var result = KeyFormatter.SanitizeKey(oldKey);
            
            // Assert
            result.Should().Be(newKey);
        }
    }
}