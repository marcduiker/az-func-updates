using AutoFixture;
using AzureFunctionsUpdates.Models.Releases;
using System.Collections.Generic;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class RepositoryConfigurationBuilder
    {
        private static Fixture _fixture = new Fixture();

        public static RepositoryConfiguration BuildOne(string repositoryName)
        {
            return _fixture.Build<RepositoryConfiguration>()
                .With(r => r.RepositoryName, repositoryName)
                .Create();
        }

        public static IReadOnlyList<RepositoryConfiguration> BuildTwo(string repository1Name, string repository2Name)
        {
            return new List<RepositoryConfiguration> {
                BuildOne(repository1Name),
                BuildOne(repository2Name)
            };
        }
    }
}
