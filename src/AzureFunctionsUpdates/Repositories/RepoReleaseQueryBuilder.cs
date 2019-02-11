using AzureFunctionsUpdates.Models;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctionsUpdates.Repositories
{
    public static class RepoReleaseQueryBuilder
    {
        public static TableQuery<RepoRelease> CreateQueryForPartitionKey(string partitionKey)
        {
            return new TableQuery<RepoRelease>()
                .Where(GetFilterConditionWhichEqualsPartitionKey(partitionKey));
        }

        private static string GetFilterConditionWhichEqualsPartitionKey(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey);
        }

        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";
    }
}
