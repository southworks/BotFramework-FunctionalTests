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
        private const string DatabaseId = "CosmosDbStorageTests";
        private const string CollectionId = "Storage";
        private const string PartitionedCollectionId = "PartitionedStorage";

        private readonly StoreItem _itemToTest = new StoreItem() { MessageList = new string[] { "hi", "how are u" }, City = "Contoso" };

        private IStorage _storage;

        private CosmosDbFixture _cosmosDbFixture;

        public CosmosDbStorageTests(CosmosDbFixture cosmosDbFixture)
        {
            _cosmosDbFixture = cosmosDbFixture;

            _storage = new CosmosDbStorage(new CosmosDbStorageOptions
            {
                AuthKey = _cosmosDbFixture.AuthKey,
                CollectionId = CollectionId,
                CosmosDBEndpoint = new Uri(_cosmosDbFixture.ServiceEndpoint),
                DatabaseId = DatabaseId,
            });
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;

            //var client = new DocumentClient(new Uri(_cosmosDbFixture.ServiceEndpoint), _cosmosDbFixture.AuthKey);
            //try
            //{
            //    await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(CosmosDatabaseName)).ConfigureAwait(false);
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("Error cleaning up resources: {0}", ex.ToString());
            //}
        }

        [Fact]
        public async Task CreateStoreItem()
        {
            // Create store item
            var items = new Dictionary<string, object>
            {
                { "createItem", _itemToTest },
            };

            await _storage.WriteAsync(items);

            var createdStoreItems = await _storage.ReadAsync(items.Keys.ToArray());

            Assert.Single(createdStoreItems);
            Assert.IsType<StoreItem>(createdStoreItems.First().Value);

            var createdStoreItem = createdStoreItems.First().Value as StoreItem;
            Assert.NotNull(createdStoreItem.ETag);
            Assert.Equal(_itemToTest.City, createdStoreItem.City);
            Assert.Equal(_itemToTest.MessageList, createdStoreItem.MessageList);
        }

        [Fact]
        public async Task UpdateStoreItem()
        {
            // Create store item
            var items = new Dictionary<string, object>
            {
                { "updateItem", _itemToTest },
            };

            await _storage.WriteAsync(items);

            var createdStoreItems = await _storage.ReadAsync(items.Keys.ToArray());
            var createdStoreItem = createdStoreItems.First().Value as StoreItem;

            // Update store item
            var list = createdStoreItem.MessageList.ToList();
            list.Add("new message");
            createdStoreItem.MessageList = list.ToArray();

            await _storage.WriteAsync(createdStoreItems);

            var updatedStoreItems = await _storage.ReadAsync(items.Keys.ToArray());

            var updatedStoreItem = updatedStoreItems.First().Value as StoreItem;

            Assert.NotEqual(createdStoreItem.ETag, updatedStoreItem.ETag);
            Assert.Equal(3, updatedStoreItem.MessageList.Length);
            Assert.Equal(createdStoreItem.MessageList, updatedStoreItem.MessageList);
        }

        [Fact]
        public async Task ReadUnknownStoreItem()
        {
            var result = await _storage.ReadAsync(new[] { "unknown" });

            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteStoreItem()
        {
            // Create store item
            var items = new Dictionary<string, object>
            {
                { "deleteItem", _itemToTest },
            };

            await _storage.WriteAsync(items);

            var createdStoreItems = await _storage.ReadAsync(items.Keys.ToArray());
            var createdStoreItem = createdStoreItems.First().Value as StoreItem;

            // Delete store item
            await _storage.DeleteAsync(items.Keys.ToArray());

            var deletedStoreItems = await _storage.ReadAsync(items.Keys.ToArray());

            Assert.NotNull(createdStoreItem);
            Assert.Empty(deletedStoreItems);
        }

        [Fact]
        public async Task CreateStoreItemWithSpecialCharacters()
        {
            var key = "!@#$%^&*()~/\\><,.?';\"`~";
            var items = new Dictionary<string, object>
            {
                { key, _itemToTest },
            };

            await _storage.WriteAsync(items);

            var createdStoreItems = await _storage.ReadAsync(items.Keys.ToArray());
            var createdStoreItem = createdStoreItems.First().Value as StoreItem;

            Assert.Single(createdStoreItems);
            Assert.NotNull(createdStoreItem);
            Assert.NotNull(createdStoreItem.ETag);
            Assert.Equal(_itemToTest.City, createdStoreItem.City);
            Assert.Equal(_itemToTest.MessageList, createdStoreItem.MessageList);
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

        //[Fact]
        //public async Task UpdateStoreItemFromPartitionedCollection()
        //{
        //    // The WriteAsync method receive a object as a parameter then encapsulate it in a object named "document"
        //    // The partitionKeyPath must have the "document" value to properly route the values as partitionKey
        //    // <see also cref="WriteAsync(IDictionary{string, object}, CancellationToken)"/>
        //    const string partitionKeyPath = "document/city";

        //    await CreateCosmosDbWithPartitionedCollection(partitionKeyPath);

        //    // Connect to the cosmosDb created before with "Contoso" as partitionKey
        //    IStorage storage = new CosmosDbStorage(CreateCosmosDbStorageOptions(PartitionedCollectionId, _itemToTest.City));
        //    var items = new Dictionary<string, object>
        //    {
        //        { "updatePartitionedItem", _itemToTest }
        //    };

        //    await storage.WriteAsync(items);

        //    var createdStoreItems = await storage.ReadAsync(items.Keys.ToArray());
        //    var createdStoreItem = createdStoreItems.First().Value as StoreItem;

        //    // Update store item
        //    createdStoreItem.City = "New York";

        //    await storage.WriteAsync(createdStoreItems);

        //    var updatedStoreItems = await storage.ReadAsync(items.Keys.ToArray());

        //    var updatedStoreItem = updatedStoreItems.First().Value as StoreItem;

        //    Assert.NotEqual(createdStoreItem.ETag, updatedStoreItem.ETag);
        //    Assert.Equal(createdStoreItem.City, updatedStoreItem.City);
        //}

        [Fact]
        public async Task ReadStoreItemFromPartitionedCollection()
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
                { "readPartitionedItem", _itemToTest }
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

        //private Mock<IDocumentClient> GetDocumentClient()
        //{
        //    var mock = new Mock<IDocumentClient>();

        //    mock.Setup(client => client.CreateDatabaseIfNotExistsAsync(It.IsAny<Database>(), It.IsAny<RequestOptions>()))
        //        .ReturnsAsync(() =>
        //        {
        //            var database = new Database();
        //            database.SetPropertyValue("SelfLink", "dummyDB_SelfLink");
        //            return new ResourceResponse<Database>(database);
        //        });

        //    mock.Setup(client => client.CreateDocumentCollectionIfNotExistsAsync(It.IsAny<Uri>(), It.IsAny<DocumentCollection>(), It.IsAny<RequestOptions>()))
        //        .ReturnsAsync(() =>
        //        {
        //            var documentCollection = new DocumentCollection();
        //            documentCollection.SetPropertyValue("SelfLink", "dummyDC_SelfLink");
        //            return new ResourceResponse<DocumentCollection>(documentCollection);
        //        });

        //    mock.Setup(client => client.ConnectionPolicy).Returns(new ConnectionPolicy());

        //    return mock;
        //}

        internal class StoreItem : IStoreItem
        {
            [JsonProperty(PropertyName = "messageList")]
            public string[] MessageList { get; set; }

            [JsonProperty(PropertyName = "city")]
            public string City { get; set; }

            public string ETag { get; set; }
        }
    }
}
