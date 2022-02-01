// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Xunit;

namespace IntegrationTests.Azure
{
    [CosmosDb(databaseId: "CosmosDbStorageTests")]
    public class CosmosDbStorageFixture : CosmosDbFixture, IAsyncLifetime
    {
        public CosmosDbStorageFixture()
        {
            ContainerId = "Storage";
        }

        public IStorage Storage { get; private set; }

        public string ContainerId { get; private set; }

        public new async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Storage = new CosmosDbStorage(new CosmosDbStorageOptions
            {
                AuthKey = AuthKey,
                CollectionId = ContainerId,
                CosmosDBEndpoint = new Uri(ServiceEndpoint),
                DatabaseId = DatabaseId,
            });
        }
    }
}
