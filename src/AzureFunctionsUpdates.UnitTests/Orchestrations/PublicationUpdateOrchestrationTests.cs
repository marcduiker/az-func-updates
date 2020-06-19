using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Orchestrations;
using AzureFunctionsUpdates.UnitTests.TestObjectBuilders;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AzureFunctionsUpdates.UnitTests.Orchestrations
{
    public class PublicationUpdateOrchestrationTests
    {
        [Fact]
        public async Task GivenHistoryIsEmptyAndNewPublicationIsAvailable_WhenOrchestrationIsRun_ThenSaveAndUpdateShouldBeCalled()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Toggles.DoPostUpdateVariableName, "true");
            var mockContext = PublicationUpdateOrchestrationContextBuilder.BuildWithoutHistoryAndWithNewWebPublication();
            var logger = new Mock<ILogger>();
            var publicationUpdateOrchestration = new PublicationUpdateOrchestration();

            // Act
            await publicationUpdateOrchestration.Run(mockContext.Object, logger.Object);

            // Assert
            mockContext.VerifyAll();
        }
    }
}
