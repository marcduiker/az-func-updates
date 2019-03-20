using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Activities.Publications;
using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Models.Publications;
using Microsoft.Azure.WebJobs;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class PublicationUpdateOrchestrationContextBuilder
    {
        public static Mock<DurableOrchestrationContextBase> BuildWithoutHistoryAndWithNewWebPublication()
        {
            var mockContext = new Mock<DurableOrchestrationContextBase>(MockBehavior.Strict);
            const string publicationSourceName = "Azure Service updates";

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<PublicationConfiguration>>(
                    nameof(GetPublicationConfigurations),
                    It.IsAny<RetryOptions>(),
                    null))
                .ReturnsAsync(PublicationConfigurationBuilder.BuildListWithOne(publicationSourceName));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<Publication>(
                    nameof(GetLatestPublicationFromHistory),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<PublicationConfiguration>()))
                .ReturnsAsync(PublicationBuilder.BuildNullPublication(publicationSourceName));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<Publication>(
                    nameof(GetLatestPublicationFromWeb),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<PublicationConfiguration>()))
                .ReturnsAsync(PublicationBuilder.BuildPublicationFromWeb(publicationSourceName));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                    nameof(SaveLatestPublication),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<Publication>()))
                .Returns(Task.CompletedTask);

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync(
                    nameof(PostUpdate),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<UpdateMessage>()))
                .Returns(Task.CompletedTask);

            return mockContext;
        }
    }
}