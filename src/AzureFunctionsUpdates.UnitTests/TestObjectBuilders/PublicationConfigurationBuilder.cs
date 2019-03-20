using AutoFixture;
using AzureFunctionsUpdates.Models.Publications;
using System.Collections.Generic;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class PublicationConfigurationBuilder
    {
        private static Fixture _fixture = new Fixture();

        private static PublicationConfiguration BuildOne(string publicationSourceName)
        {
            return _fixture.Build<PublicationConfiguration>()
               .With(p => p.PublicationSourceName, publicationSourceName)
               .Create();
        }

        public static IReadOnlyList<PublicationConfiguration> BuildListWithOne(string publicationSourceName)
        {
            return new List<PublicationConfiguration> { BuildOne(publicationSourceName) };
        }
    }
}
