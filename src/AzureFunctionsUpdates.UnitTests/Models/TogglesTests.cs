using AzureFunctionsUpdates.Models;
using FluentAssertions;
using System;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Models
{
    public class TogglesTests
    {
        [Fact]
        public void GivenToggleDoPostUpdateEnvironmentVariableIsSetToNull_WhenRetrievingDoPostUpdate_ThenResultShouldBeFalse()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, null);

            // Act
            var result = Toggles.DoPostUpdate;

            // Assert
            result.Should().BeFalse($"because the {Toggles.DoPostUpdateVariableName} environment variable variable is not set.");
        }

        [Fact]
        public void GivenToggleDoPostUpdateEnvironmentVariableIsSetToIncorrectValue_WhenRetrievingDoPostUpdate_ThenResultShouldBeFalse()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "notabooleanvalue");

            // Act
            var result = Toggles.DoPostUpdate;

            // Assert
            result.Should().BeFalse($"because the {Toggles.DoPostUpdateVariableName} environment variable is not a valid boolean value.");
        }

        [Fact]
        public void GivenToggleDoPostUpdateEnvironmentVariableIsSetToFalse_WhenRetrievingDoPostUpdate_ThenResultShouldBeFalse()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "false");

            // Act
            var result = Toggles.DoPostUpdate;

            // Assert
            result.Should().BeFalse($"because the {Toggles.DoPostUpdateVariableName} environment variable is not a valid boolean value.");
        }

        [Fact]
        public void GivenToggleDoPostUpdateEnvironmentVariableIsSetToTrue_WhenRetrievingDoPostUpdate_ThenResultShouldBeTrue()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");

            // Act
            var result = Toggles.DoPostUpdate;

            // Assert
            result.Should().BeTrue($"because the {Toggles.DoPostUpdateVariableName} environment variable is set to true value.");
        }
    }
}
