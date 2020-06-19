using AzureFunctionsUpdates.Models.RepositoryReleases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctionsUpdates.Builders
{
    public static class LatestObjectsBuilder
    {        
        public static TLatest Build<TConfiguration, TRelease, TLatest>(
            TConfiguration configuration,
            IEnumerable<TRelease> newObjects,
            IEnumerable<TRelease> historicalObjects,
            Func<TConfiguration, TRelease, bool> matchObject)
            where TLatest : new()
            
        {
            var latestNew = newObjects.First(obj => matchObject(configuration, obj));
            var latestHistorical = historicalObjects.First(obj => matchObject(configuration, obj));

            return (TLatest)Activator.CreateInstance(typeof(TLatest), latestNew, latestHistorical);
        }
    }
}