namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class GitHubNullRelease : NullRelease
    {
        public GitHubNullRelease(string repositoryName) :
            base(repositoryName)
        { } 
    }
}
