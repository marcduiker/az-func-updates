using AutoFixture;
using AzureFunctionsUpdates.Models.Publications;
using System;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class PublicationBuilder
    {
        private static Fixture _fixture = new Fixture();

        public static Publication BuildNullPublication(string publicationSourceName)
        {
            return new NullPublication(publicationSourceName);
        }

        public static Publication BuildPublicationFromWeb(string publicationSourceName)
        {
            return _fixture.Build<Publication>()
                .With(p => p.PublicationSourceName, publicationSourceName)
                .With(p => p.PublicationDate, DateTimeOffset.Now)
                .Create();   
        }
    }
}
