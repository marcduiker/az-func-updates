using System;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.Builders
{
    public static class ReleaseFunctionBuilder
    {
        public static Func<RepositoryConfiguration, RepositoryRelease, bool> BuildForMatchingRepositoryName()
        {
            return (config, release) => release.RepositoryName.Equals(
                config.RepositoryName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}