using AutoFixture;
using System;
using System.Collections.Generic;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.UnitTests.TestObjectBuilders
{
    public static class RepositoryReleaseBuilder
    {
        private static IFixture _fixture = new Fixture();

        public static T BuildOne<T>(string repositoryName) 
            where T : RepositoryRelease
        {
            return _fixture.Build<T>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseCreatedAt, DateTimeOffset.Now)
                .Create();
        }

        public static T BuildOneWithReleaseId<T>(string repositoryName, int id) 
            where T : RepositoryRelease
        {
            return _fixture.Build<T>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseCreatedAt, DateTimeOffset.Now)
                .With(r => r.ReleaseId, id)
                .Create();
        }

        public static T BuildOneWithReleaseIdAndDate<T>(string repositoryName, int id, DateTimeOffset releaseDate)
            where T : RepositoryRelease
        {
            return _fixture.Build<T>()
                .With(r => r.RepositoryName, repositoryName)
                .With(r => r.ReleaseId, id)
                .With(r => r.ReleaseCreatedAt, releaseDate)
                .Create();
        }
        
        public static T BuildOneWithLongRepoAndReleaseName<T>()
            where T : RepositoryRelease
        {
            return _fixture.Build<T>()
                .With(r => r.RepositoryName, "azure-functions-powershell-worker")
                .With(r => r.ReleaseName, "v0.1.174 Release of PowerShell worker for Azure Functions")
                .With(r => r.TagName, " v0.1.174-preview")
                .With( r=> r.HtmlUrl, "https://github.com/Azure/azure-functions-powershell-worker/releases/tag/v0.1.174-preview")
                .With(r => r.HashTags, "#AzureFunctions #Serverless #PowerShell")
                .Create();
        }
        
        public static T BuildOneWithShortRepoAndReleaseNameOnAspecificDate<T>()
            where T : RepositoryRelease
        {
            return _fixture.Build<T>()
                .With(r => r.RepositoryName, "azure-functions-host")
                .With(r => r.ReleaseName, "Azure Functions Runtime 2.0.12477")
                .With(r => r.ReleaseCreatedAt, new DateTime(2018, 1, 1))
                .With(r => r.TagName, "v2.0.12477")
                .With(r=> r.HtmlUrl, "https://github.com/Azure/azure-functions-host/releases/tag/v2.0.12477")
                .With(r => r.HashTags, "#AzureFunctions #Serverless")
                .Create();
        }

        public static T BuildNullRelease<T>(string repositoryName)
            where T : RepositoryRelease
        {
            var release =_fixture.Build<T>()
                .With(r => r.RepositoryName, repositoryName)
                .Without(r => r.PartitionKey)
                .Create();

            return release;
        }

        public static IReadOnlyList<RepositoryRelease> BuildListContainingOneWithReleaseId<T>(string repositoryName, int id)
            where T : RepositoryRelease
        {
            return new List<RepositoryRelease>
            {
                BuildOneWithReleaseId<T>(repositoryName, id)
            };
        }

        public static IReadOnlyList<RepositoryRelease> BuildListContainingOneWithReleaseIdAndDate<T>(string repositoryName, int id, DateTimeOffset releaseDate)
            where T : RepositoryRelease
        {
            return new List<RepositoryRelease>
            {
                BuildOneWithReleaseIdAndDate<T>(repositoryName, id, releaseDate)
            };
        }

        public static IReadOnlyList<RepositoryRelease> BuildListContainingOneNullRelease<T>(string repositoryName)
            where T : RepositoryRelease
        {
            return new List<RepositoryRelease>
            {
                BuildNullRelease<T>(repositoryName)
            };
        }
    }
}
