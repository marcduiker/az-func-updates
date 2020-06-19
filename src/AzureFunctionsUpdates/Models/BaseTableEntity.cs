
using System;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctionsUpdates.Models
{
    public class BaseConfiguration : TableEntity
    {
        public string HashTags { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
