using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctionsUpdates.Storage
{
    public static class QueryBuilder<T> where T : TableEntity, new()
    {
        public static TableQuery<T> CreateQueryForPartitionKey(string partitionKey)
        {
            var sanitizedPartitionKey = KeyFormatter.SanitizeKey(partitionKey);
            return new TableQuery<T>()
                .Where(GetFilterConditionWhichEqualsPartitionKey(sanitizedPartitionKey));
        }

        private static string GetFilterConditionWhichEqualsPartitionKey(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey);
        }

        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";
    }
}
