using AutoFixture;
using System;
using System.Collections.Generic;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class RepositoryConfigurationBuilder
    {
        private static readonly Fixture _fixture = new Fixture();

        public static RepositoryConfiguration BuildOne(string repositoryName)
        {
            return _fixture.Build<RepositoryConfiguration>()
                .With(r => r.RepositoryName, repositoryName)
                .Create();
        }

        public static IReadOnlyList<RepositoryConfiguration> BuildListWithOne(string repositoryName)
        {
            return new List<RepositoryConfiguration>
            {
                BuildOne(repositoryName)
            };
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
