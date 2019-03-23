using AutoFixture;
using System;
using System.Collections.Generic;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class RepositoryReleaseBuilder
    {
        private static Fixture _fixture = new Fixture();

        public static RepositoryRelease BuildOne(string repositoryName)
        {
            return _fixture.Build<RepositoryRelease>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseCreatedAt, DateTimeOffset.Now)
                .Create();
        }

        public static RepositoryRelease BuildOneWithReleaseId(string repositoryName, int id)
        {
            return _fixture.Build<RepositoryRelease>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseCreatedAt, DateTimeOffset.Now)
                .With(r => r.ReleaseId, id)
                .Create();
        }

        public static RepositoryRelease BuildOneWithReleaseIdAndDate(string repositoryName, int id, DateTimeOffset releaseDate)
        {
            return _fixture.Build<RepositoryRelease>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseId, id)
                .With(r => r.ReleaseCreatedAt, releaseDate)
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

        public static IReadOnlyList<RepositoryRelease> BuildListContainingOneWithReleaseIdAndDate(string repositoryName, int id, DateTimeOffset releaseDate)
        {
            return new List<RepositoryRelease>
            {
                BuildOneWithReleaseIdAndDate(repositoryName, id, releaseDate)
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
