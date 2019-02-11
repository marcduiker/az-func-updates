using AzureFunctionsUpdates.Exceptions;
using AzureFunctionsUpdates.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsUpdates.Repositories
{
    public class RepoReleaseRepository
    {
            private readonly CloudTableClient _cloudTableClient;
            public const string TableName = "RepoReleases";

            public RepoReleaseRepository(CloudTableClient cloudTableClient)
            {
                _cloudTableClient = cloudTableClient;
            }

            public async Task<IReadOnlyCollection<RepoRelease>> GetAllAsync(string partitionKey)
            {
                var table = GetCloudTable();
                var query = RepoReleaseQueryBuilder.CreateQueryForPartitionKey(partitionKey);
                var result = await QueryRepoReleasesAsync(table, query);

                return result;
            }

            public async Task<RepoRelease> GetByRowKeyAsync(string partitionKey, string rowKey)
            {
                var table = GetCloudTable();
                RepoRelease ruleResult = null;
                var retrieveOperation = TableOperation.Retrieve<RepoRelease>(partitionKey, rowKey);
                try
                {
                    var retrievedResult = await table.ExecuteAsync(retrieveOperation);

                    if (retrievedResult.Result != null)
                    {
                        ruleResult = (RepoRelease)retrievedResult.Result;
                    }
                }
                catch (Exception e)
                {
                    throw ExceptionBuilder.CreateExceptionForTableOperation(partitionKey, rowKey, table, retrieveOperation, e);
                }

                return ruleResult;
            }

            public async Task CreateAsync(RepoRelease rule)
            {
                var table = GetCloudTable();
                var insertOperation = TableOperation.Insert(rule);
                try
                {
                    await table.ExecuteAsync(insertOperation);
                }
                catch (Exception e)
                {
                    throw ExceptionBuilder.CreateExceptionForTableOperation(rule, table, insertOperation, e);
                }
            }

            public async Task UpdateAsync(RepoRelease rule)
            {
                var table = GetCloudTable();
                var updateOperation = TableOperation.InsertOrReplace(rule);
                try
                {
                    await table.ExecuteAsync(updateOperation);
                }
                catch (Exception e)
                {
                    throw ExceptionBuilder.CreateExceptionForTableOperation(rule, table, updateOperation, e);
                }
            }

            public async Task DeleteAsync(RepoRelease rule)
            {
                var table = GetCloudTable();
                var deleteOperation = TableOperation.Delete(rule);
                try
                {
                    await table.ExecuteAsync(deleteOperation);
                }
                catch (Exception e)
                {
                    throw ExceptionBuilder.CreateExceptionForTableOperation(rule, table, deleteOperation, e);
                }
            }
            private async Task<IReadOnlyCollection<RepoRelease>> QueryRepoReleasesAsync(CloudTable table, TableQuery<RepoRelease> query)
            {
                IEnumerable<RepoRelease> result;

                try
                {
                    var queryResult = table.ExecuteQuery(query);
                    result = await Task.FromResult(queryResult);
                }
                catch (Exception e)
                {
                    throw ExceptionBuilder.CreateExceptionForTableQuery(table, query, e);
                }

                return result.ToList();
            }

            private CloudTable GetCloudTable()
            {
                var cloudTable = _cloudTableClient.GetTableReference(TableName);

                try
                {
                    cloudTable.CreateIfNotExists();
                }
                catch (Exception e)
                {
                    throw ExceptionBuilder.CreateExceptionForTableCreation(cloudTable, e);
                }

                return cloudTable;
            }
        
    }
}
