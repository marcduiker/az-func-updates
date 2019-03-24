using System;
using System.Collections.Generic;
using System.Linq;
using AzureFunctionsUpdates.Models.RepositoryReleases;

namespace AzureFunctionsUpdates.Builders
{
    public static class LatestObjectsBuilder
    {        
        public static TLatest Build<TConfiguration, TMonitored, TLatest>(
            TConfiguration configuration,
            IEnumerable<TMonitored> newObjects,
            IEnumerable<TMonitored> historicalObjects,
            Func<TConfiguration, TMonitored, bool> matchObject) where TLatest : new()
        {
            var latestNew = newObjects.First(obj => matchObject(configuration, obj));
            var latestHistorical = historicalObjects.First(obj => matchObject(configuration, obj));

            return (TLatest)Activator.CreateInstance(typeof(TLatest), latestNew, latestHistorical);
        }
    }
}