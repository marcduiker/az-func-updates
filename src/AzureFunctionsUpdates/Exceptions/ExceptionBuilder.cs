using Microsoft.Azure.Cosmos.Table;
using System;

namespace AzureFunctionsUpdates.Exceptions
{
    public static class ExceptionBuilder
    {
        private const string ErrorPrefix = "An error occurred while";

        public static RepoException CreateExceptionForTableCreation(CloudTable table, Exception exception)
        {
            return new RepoException($"{ErrorPrefix} creating table {table.Name}.", exception);
        }

        public static RepoException CreateExceptionForTableOperation(ITableEntity entity, CloudTable table, TableOperation tableOperation, Exception exception)
        {
            return new RepoException($"{ErrorPrefix} performing a {tableOperation.OperationType.ToString()} operation on table {table.Name} with partitionKey {entity.PartitionKey} and rowKey {entity.RowKey}.", exception);
        }

        public static RepoException CreateExceptionForTableOperation(string partitionKey, string rowKey, CloudTable table, TableOperation tableOperation, Exception exception)
        {
            return new RepoException($"{ErrorPrefix} performing a {tableOperation.OperationType.ToString()} operation on table {table.Name} with partitionKey {partitionKey} and rowKey {rowKey}.", exception);
        }

        public static RepoException CreateExceptionForTableQuery<T>(CloudTable table, TableQuery<T> query, Exception exception)
        {
            return new RepoException($"{ErrorPrefix} querying table {table.Name} with {query.FilterString}.", exception);
        }
    }
}
