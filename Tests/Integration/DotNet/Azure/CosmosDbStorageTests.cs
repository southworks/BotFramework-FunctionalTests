// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Azure
{
    [Collection("CosmosDb")]
    [Trait("TestCategory", "Storage")]
    [Trait("TestCategory", "Storage - CosmosDB")]
    [Trait("TestCategory", "Deprecated")]
    public class CosmosDbStorageTests : StorageBaseTests, IAsyncLifetime
    {
        private const string CosmosDatabaseName = "test-CosmosDbStorageTests";
        private const string CosmosCollectionName = "bot-storage";

        private IStorage _storage;

        private CosmosDbFixture _cosmosDbFixture;

        public CosmosDbStorageTests(CosmosDbFixture cosmosDbFixture)
        {
            _cosmosDbFixture = cosmosDbFixture;

            _storage = new CosmosDbStorage(new CosmosDbStorageOptions
            {
                AuthKey = _cosmosDbFixture.AuthKey,
                CollectionId = CosmosCollectionName,
                CosmosDBEndpoint = new Uri(_cosmosDbFixture.ServiceEndpoint),
                DatabaseId = CosmosDatabaseName,
            });
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            var client = new DocumentClient(new Uri(_cosmosDbFixture.ServiceEndpoint), _cosmosDbFixture.AuthKey);
            try
            {
                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(CosmosDatabaseName)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error cleaning up resources: {0}", ex.ToString());
            }
        }

        [Fact]
        public async Task CreateObjectCosmosDBTest()
        {
            await CreateObjectTest(_storage);
        }
    }
}
