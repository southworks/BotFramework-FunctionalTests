// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IntegrationTests.Azure
{
    [Trait("TestCategory", "Storage")]
    [Trait("TestCategory", "Storage - CosmosDB")]
    [Trait("TestCategory", "Deprecated")]
    public class CosmosDbStorageTests : StorageBaseTests, IClassFixture<CosmosDbStorageFixture>
    {
        private readonly StoreItem _itemToTest = new StoreItem() { MessageList = new string[] { "hi", "how are u" }, City = "Contoso" };

        private readonly CosmosDbStorageFixture _cosmosDbFixture;

        public CosmosDbStorageTests(CosmosDbStorageFixture cosmosDbFixture)
        {
            _cosmosDbFixture = cosmosDbFixture;
        }

        [Fact]
        public Task CreateStoreItem()
        {
            return CreateStoreItemTest(_cosmosDbFixture.Storage);
        }

        [Fact]
        public Task UpdateStoreItem()
        {
            return UpdateStoreItemTest(_cosmosDbFixture.Storage);
        }

        [Fact]
        public Task ReadUnknownStoreItem()
        {
            return ReadUnknownStoreItemTest(_cosmosDbFixture.Storage);
        }

        [Fact]
        public Task DeleteStoreItem()
        {
            return DeleteStoreItemTest(_cosmosDbFixture.Storage);
        }

        [Fact]
        public Task CreateStoreItemWithSpecialCharacters()
        {
            return CreateStoreItemWithSpecialCharactersTest(_cosmosDbFixture.Storage);
        }

        [Fact]
        public async Task DeleteStoreItemFromPartitionedCollection()
        {
            // The WriteAsync method receive an object as a parameter then encapsulate it in an object named "document"
            // The partitionKeyPath must have the "document" value to properly route the values as partitionKey
            // <see also cref="WriteAsync(IDictionary{string, object}, CancellationToken)"/>
            const string partitionKeyPath = "document/city";

            await CreateCosmosDbWithPartitionedCollection(partitionKeyPath);

            IStorage storage = new CosmosDbStorage(CreateCosmosDbStorageOptions(PartitionedCollectionId, _itemToTest.City));
            var items = new Dictionary<string, object>
            {
                { "deletePartitionedItem", _itemToTest }
            };

            await storage.WriteAsync(items);

            // Delete store item
            await storage.DeleteAsync(items.Keys.ToArray());
            var deletedStoreItems = await storage.ReadAsync(items.Keys.ToArray());

            Assert.Empty(deletedStoreItems);
        }

        [Fact]
        public async Task UpdateStoreItemFromPartitionedCollection()
        {
            // The WriteAsync method receive a object as a parameter then encapsulate it in a object named "document"
            // The partitionKeyPath must have the "document" value to properly route the values as partitionKey
            // <see also cref="WriteAsync(IDictionary{string, object}, CancellationToken)"/>
            const string partitionKeyPath = "document/city";

            await CreateCosmosDbWithPartitionedCollection(partitionKeyPath);

            // Connect to the cosmosDb created before with "Contoso" as partitionKey
            IStorage storage = new CosmosDbStorage(CreateCosmosDbStorageOptions(PartitionedCollectionId, _itemToTest.City));
            var items = new Dictionary<string, object>
            {
                { "updatePartitionedItem", _itemToTest }
            };

            await storage.WriteAsync(items);

            var createdStoreItems = await storage.ReadAsync(items.Keys.ToArray());
            var createdStoreItem = createdStoreItems.First().Value as StoreItem;

            // Update store item
            createdStoreItem.MessageList = new string[] { "new message" };

            await storage.WriteAsync(createdStoreItems);

            var updatedStoreItems = await storage.ReadAsync(items.Keys.ToArray());

            var updatedStoreItem = updatedStoreItems.First().Value as StoreItem;

            Assert.NotEqual(createdStoreItem.ETag, updatedStoreItem.ETag);
            Assert.Equal(createdStoreItem.MessageList, updatedStoreItem.MessageList);
        }

        [Fact]
        public async Task CreateStoreItemFromPartitionedCollection()
        {
            // The WriteAsync method receive a object as a parameter then encapsulate it in a object named "document"
            // The partitionKeyPath must have the "document" value to properly route the values as partitionKey
            // <see also cref="WriteAsync(IDictionary{string, object}, CancellationToken)"/>
            const string partitionKeyPath = "document/city";

            await CreateCosmosDbWithPartitionedCollection(partitionKeyPath);

            // Connect to the cosmosDb created before with "Contoso" as partitionKey
            IStorage storage = new CosmosDbStorage(CreateCosmosDbStorageOptions(PartitionedCollectionId, _itemToTest.City));
            var items = new Dictionary<string, object>
            {
                { "createPartitionedItem", _itemToTest }
            };

            await storage.WriteAsync(items);
            var storeItems = await storage.ReadAsync<StoreItem>(items.Keys.ToArray());

            Assert.Equal(_itemToTest.City, storeItems.First().Value.City);
        }

        private async Task CreateCosmosDbWithPartitionedCollection(string partitionKey)
        {
            using var client = new DocumentClient(new Uri(_cosmosDbFixture.ServiceEndpoint), _cosmosDbFixture.AuthKey);
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            var partitionKeyDefinition = new PartitionKeyDefinition { Paths = new Collection<string> { $"/{partitionKey}" } };
            var collectionDefinition = new DocumentCollection { Id = PartitionedCollectionId, PartitionKey = partitionKeyDefinition };

            await client.CreateDocumentCollectionIfNotExistsAsync(database.SelfLink, collectionDefinition);
        }

        private CosmosDbStorageOptions CreateCosmosDbStorageOptions(string collectionId, string partitionKey = "")
        {
            return new CosmosDbStorageOptions()
            {
                PartitionKey = partitionKey,
                AuthKey = _cosmosDbFixture.AuthKey,
                CollectionId = collectionId,
                CosmosDBEndpoint = new Uri(_cosmosDbFixture.ServiceEndpoint),
                DatabaseId = DatabaseId,
            };
        }
    }
}
