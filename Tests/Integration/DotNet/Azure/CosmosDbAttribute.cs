using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Azure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CosmosDbAttribute : Attribute
    {
        public CosmosDbAttribute(string databaseId = "CosmosDatabase", string containerId = "CosmosContainer")
        {
            DatabaseId = databaseId;
            ContainerId = containerId;
        }

        public string DatabaseId { get; private set; }

        public string ContainerId { get; private set; }
    }
}
