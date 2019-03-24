﻿using System;

namespace AzureFunctionsUpdates.Models.RepositoryReleases
{
    public class NullRelease : RepositoryRelease
    {
        public NullRelease(string repositoryName) :
            base(
                repositoryName,
                default(int),
                default(string),
                default(string),
                default(DateTimeOffset),
                default(string),
                default(string),
                default(string))
        { } 
    }
}
