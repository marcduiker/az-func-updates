using AutoFixture;
using AzureFunctionsUpdates.Models;
using System.Collections.Generic;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class RepositoryReleaseBuilder
    {
        private static Fixture _fixture = new Fixture();

        public static RepositoryRelease BuildOne(string repositoryName)
        {
            return _fixture.Build<RepositoryRelease>()
                .With(r => r.RepositoryName, repositoryName)
                .Create();
        }

        public static RepositoryRelease BuildOneWithReleaseId(string repositoryName, int id)
        {
            return _fixture.Build<RepositoryRelease>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseId, id)
                .Create();
        }

        public static RepositoryRelease BuildNullRelease(string repositoryName)
        {
            return new NullRelease(repositoryName);
        }

        public static IReadOnlyList<RepositoryRelease> BuildListContainingOneWithReleaseId(string repositoryName, int id)
        {
            return new List<RepositoryRelease>
            {
                BuildOneWithReleaseId(repositoryName, id)
            };
        }

        public static IReadOnlyList<RepositoryRelease> BuildListContainingOneNullRelease(string repositoryName)
        {
            return new List<RepositoryRelease>
            {
                BuildNullRelease(repositoryName)
            };
        }
    }
}
