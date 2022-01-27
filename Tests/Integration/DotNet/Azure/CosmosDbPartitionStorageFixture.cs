// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace IntegrationTests.Azure
{
    public class CosmosDbPartitionStorageFixture : IAsyncLifetime
    {
        public const string CosmosDatabaseName = "test-CosmosDbPartitionStorageTests";
        public const string CosmosCollectionName = "bot-storage";

        private readonly CosmosDbFixture _cosmosDbFixture;
        private CosmosClient client;

        public CosmosDbPartitionStorageFixture(CosmosDbFixture cosmosDbFixture)
        {
            _cosmosDbFixture = cosmosDbFixture;
        }

        public async Task InitializeAsync()
        {
            client = new CosmosClient(
                _cosmosDbFixture.ServiceEndpoint,
                _cosmosDbFixture.AuthKey,
                new CosmosClientOptions());

            await client.CreateDatabaseIfNotExistsAsync(CosmosDatabaseName);
        }

        public async Task DisposeAsync()
        {
            try
            {
                await client.GetDatabase(CosmosDatabaseName).DeleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error cleaning up resources: {0}", ex.ToString());
            }
        }
    }
}
