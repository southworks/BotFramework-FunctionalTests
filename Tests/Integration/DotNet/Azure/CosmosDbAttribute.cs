using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Azure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CosmosDbAttribute : Attribute
    {
        public CosmosDbAttribute(string databaseId)
        {
            DatabaseId = databaseId;
        }

        public string DatabaseId { get; private set; }
    }
}
