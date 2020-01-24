using AzureFunctionsUpdates.Activities;
using AzureFunctionsUpdates.Activities.Publications;
using AzureFunctionsUpdates.Models;
using AzureFunctionsUpdates.Models.Publications;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class PublicationUpdateOrchestrationContextBuilder
    {
        public static Mock<IDurableOrchestrationContext> BuildWithoutHistoryAndWithNewWebPublication()
        {
            var mockContext = new Mock<IDurableOrchestrationContext>(MockBehavior.Strict);
            const string publicationSourceName = "Azure Service updates";

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<IReadOnlyList<PublicationConfiguration>>(
                    nameof(GetPublicationConfigurationsActivity),
                    It.IsAny<RetryOptions>(),
                    null))
                .ReturnsAsync(PublicationConfigurationBuilder.BuildListWithOne(publicationSourceName));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<Publication>(
                    nameof(GetLatestPublicationFromHistoryActivity),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<PublicationConfiguration>()))
                .ReturnsAsync(PublicationBuilder.BuildNullPublication(publicationSourceName));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<Publication>(
                    nameof(GetLatestPublicationFromWebActivity),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<PublicationConfiguration>()))
                .ReturnsAsync(PublicationBuilder.BuildPublicationFromWeb(publicationSourceName));

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                    nameof(SaveLatestPublicationActivity),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<Publication>()))
                .ReturnsAsync(true);

            mockContext
                .Setup(c => c.CallActivityWithRetryAsync<bool>(
                    nameof(PostUpdateActivity),
                    It.IsAny<RetryOptions>(),
                    It.IsAny<UpdateMessage>()))
                .ReturnsAsync(true);

            return mockContext;
        }
    }
}