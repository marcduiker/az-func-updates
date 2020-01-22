namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public abstract class NullRelease : RepositoryRelease
    {
        public NullRelease(string repositoryName) :
            base(
                repositoryName,
                default,
                default,
                default,
                default,
                default,
                default,
                default)
        { } 
    }
}
