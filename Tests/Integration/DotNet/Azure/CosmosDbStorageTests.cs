// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Xunit;

namespace IntegrationTests.Azure
{
    [Trait("TestCategory", "Storage")]
    [Trait("TestCategory", "CosmosDB")]
    [Trait("TestCategory", "Deprecated")]
    public class CosmosDbStorageTests : StorageBaseTests, IClassFixture<CosmosDbStorageFixture>
    {
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
            await _cosmosDbFixture.CreateStoragePartitionedContainer("document/city");
            var storage = _cosmosDbFixture.GetStoragePartitionedContainer(StoreItemSample.City);

            var items = new Dictionary<string, object>
            {
                { "deletePartitionedItem", StoreItemSample }
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
            await _cosmosDbFixture.CreateStoragePartitionedContainer("document/city");
            var storage = _cosmosDbFixture.GetStoragePartitionedContainer(StoreItemSample.City);

            var items = new Dictionary<string, object>
            {
                { "updatePartitionedItem", StoreItemSample }
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
            await _cosmosDbFixture.CreateStoragePartitionedContainer("document/city");
            var storage = _cosmosDbFixture.GetStoragePartitionedContainer(StoreItemSample.City);

            var items = new Dictionary<string, object>
            {
                { "createPartitionedItem", StoreItemSample }
            };

            await storage.WriteAsync(items);
            var storeItems = await storage.ReadAsync<StoreItem>(items.Keys.ToArray());

            Assert.Equal(StoreItemSample.City, storeItems.First().Value.City);
        }
    }
}
