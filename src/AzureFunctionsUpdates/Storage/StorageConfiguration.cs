namespace AzureFunctionsUpdates.Storage
{
    public class Configuration
    {
        public const string ConnectionName = "TableStorageConnection";

        public class Releases
        {
            public const string TableName = "AzFuncUpdatesReleases";
        }

        public class Publications
        {
            public const string TableName = "AzFuncUpdatesPublications";
        }

        public class RepositoryConfigurations
        {
            public const string TableName = "AzFuncUpdatesConfigurations";
            public const string PartitionKey = "Repositories";
        }

        public class PublicationConfigurations
        {
            public const string TableName = "AzFuncUpdatesConfigurations";
            public const string PartitionKey = "Publications";
        }
    }
}
