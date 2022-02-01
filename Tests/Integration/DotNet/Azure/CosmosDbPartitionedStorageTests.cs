// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IntegrationTests.Azure
{
    //[Collection("CosmosDb")]
    [Trait("TestCategory", "Storage")]
    [Trait("TestCategory", "Storage - CosmosDB Partitioned")]
    public class CosmosDbPartitionedStorageTests : StorageBaseTests, IAsyncLifetime, IClassFixture<CosmosDbPartitionStorageFixture>
    {
        // Endpoint and Authkey for the CosmosDB Emulator running locally
        private const string CosmosServiceEndpoint = "https://localhost:8081";
        private const string CosmosAuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string DatabaseId = "CosmosDbPartitionStorageTests";
        private const string CosmosCollectionName = "bot-storage";
        private const string PartitionedCollectionId = "PartitionedStorage";
        private IStorage _storage;
        private readonly StoreItem _itemToTest = new StoreItem() { MessageList = new string[] { "hi", "how are u" }, City = "Contoso" };

        private CosmosDbFixture _cosmosDbFixture;

        public CosmosDbPartitionedStorageTests(CosmosDbPartitionStorageFixture cosmosDbFixture)
        {
            _cosmosDbFixture = cosmosDbFixture;

            _storage = new CosmosDbPartitionedStorage(
                new CosmosDbPartitionedStorageOptions
                {
                    AuthKey = CosmosAuthKey,
                    ContainerId = CosmosCollectionName,
                    CosmosDbEndpoint = CosmosServiceEndpoint,
                    DatabaseId = DatabaseId,
                });
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _storage = null;

            return Task.CompletedTask;
        }

        [Fact]
        public Task CreateStoreItem()
        {
            return CreateStoreItemTest(_storage);
        }

        [Fact]
        public Task UpdateStoreItem()
        {
            return UpdateStoreItemTest(_storage);
        }

        [Fact]
        public Task ReadUnknownStoreItem()
        {
            return ReadUnknownStoreItemTest(_storage);
        }

        [Fact]
        public Task DeleteStoreItem()
        {
            return DeleteStoreItemTest(_storage);
        }

        [Fact]
        public Task CreateStoreItemWithSpecialCharacters()
        {
            return CreateStoreItemWithSpecialCharactersTest(_storage);
        }

        [Fact]
        public async Task CreateStoreItemWithNestingLimit()
        {
            async Task TestNestAsync(int depth)
            {
                // This creates nested data with both objects and arrays
                static JToken CreateNestedData(int count, bool isArray = false)
                    => count > 0
                        ? (isArray
                            ? new JArray { CreateNestedData(count - 1, false) } as JToken
                            : new JObject { new JProperty("data", CreateNestedData(count - 1, true)) })
                        : null;

                var dict = new Dictionary<string, object>
                {
                    { "nestingLimit", CreateNestedData(depth) },
                };

                await _storage.WriteAsync(dict);
            }

            // Should not throw
            await TestNestAsync(127);

            try
            {
                // Should either not throw or throw a special exception
                await TestNestAsync(128);
            }
            catch (InvalidOperationException ex)
            {
                // If the nesting limit is changed on the Cosmos side
                // then this assertion won't be reached, which is okay
                Assert.Contains("recursion", ex.Message);
            }
        }

        [Fact]
        public async Task CreateStoreitemWithDialogsNestingLimit()
        {
            async Task TestDialogNestAsync(int dialogDepth)
            {
                Dialog CreateNestedDialog(int depth) => new ComponentDialog(nameof(ComponentDialog))
                    .AddDialog(depth > 0
                        ? CreateNestedDialog(depth - 1)
                        : new WaterfallDialog(
                            nameof(WaterfallDialog),
                            new List<WaterfallStep>
                            {
                                (stepContext, ct) => Task.FromResult(Dialog.EndOfTurn)
                            }));

                var dialog = CreateNestedDialog(dialogDepth);

                var convoState = new ConversationState(_storage);

                var adapter = new TestAdapter(TestAdapter.CreateConversation("nestingTest"))
                    .Use(new AutoSaveStateMiddleware(convoState));

                var dialogState = convoState.CreateProperty<DialogState>("dialogStateForNestingTest");

                await new TestFlow(adapter, async (turnContext, cancellationToken) =>
                {
                    if (turnContext.Activity.Text == "reset")
                    {
                        await dialogState.DeleteAsync(turnContext);
                    }
                    else
                    {
                        await dialog.RunAsync(turnContext, dialogState, cancellationToken);
                    }
                })
                    .Send("reset")
                    .Send("hello")
                    .StartTestAsync();
            }

            // Should not throw
            await TestDialogNestAsync(23);

            try
            {
                // Should either not throw or throw a special exception
                await TestDialogNestAsync(24);
            }
            catch (InvalidOperationException ex)
            {
                // If the nesting limit is changed on the Cosmos side
                // then this assertion won't be reached, which is okay
                Assert.Contains("dialogs", ex.Message);
            }
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
            Microsoft.Azure.Documents.Database database = await client.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database { Id = DatabaseId });
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
