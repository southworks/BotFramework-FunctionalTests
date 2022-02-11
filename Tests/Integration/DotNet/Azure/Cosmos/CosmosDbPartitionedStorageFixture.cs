// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationTests.Azure.Storage;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Azure.Cosmos
{
    [CosmosDb(databaseId: "CosmosDbPartitionedStorageTests")]
    public class CosmosDbPartitionedStorageFixture : CosmosDbBaseFixture, IAsyncLifetime
    {
        public IDictionary<StorageCase, IStorage> Storages { get; private set; }

        public new async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var options = new CosmosDbPartitionedStorageOptions
            {
                AuthKey = AuthKey,
                ContainerId = ContainerId,
                CosmosDbEndpoint = ServiceEndpoint,
                DatabaseId = DatabaseId,
            };

            Storages = new Dictionary<StorageCase, IStorage>
            {
                { StorageCase.Default, new CosmosDbPartitionedStorage(options) },
                { StorageCase.TypeNameHandlingNone, new CosmosDbPartitionedStorage(options, new JsonSerializer() { TypeNameHandling = TypeNameHandling.None }) }
            };
        }
    }
}
