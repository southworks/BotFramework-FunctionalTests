// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Xunit;

namespace IntegrationTests.Azure
{
    public class StorageBaseTests
    {
        public StorageBaseTests()
        {
            StoreItemSample = new StoreItem() { MessageList = new string[] { "hi", "how are u" }, City = "Contoso" };
        }

        public StoreItem StoreItemSample { get; private set; }

        protected async Task ReadUnknownStoreItemTest(IStorage storage)
        {
            var result = await storage.ReadAsync(new[] { "unknown" });

            Assert.Empty(result);
        }

        protected async Task CreateStoreItemTest(IStorage storage)
        {
            var dict = new Dictionary<string, object>
            {
                { "createItem", StoreItemSample },
            };

            await storage.WriteAsync(dict);

            var items = await storage.ReadAsync<StoreItem>(dict.Keys.ToArray());
            var item = items.FirstOrDefault().Value;

            Assert.Single(items);
            Assert.NotNull(item);
            Assert.NotNull(item.ETag);
            Assert.Equal(StoreItemSample.City, item.City);
            Assert.Equal(StoreItemSample.MessageList, item.MessageList);
        }

        protected async Task CreateStoreItemWithSpecialCharactersTest(IStorage storage)
        {
            var key = "!@#$%^&*()~/\\><,.?';\"`~";
            var dict = new Dictionary<string, object>
            {
                { key, StoreItemSample },
            };

            await storage.WriteAsync(dict);

            var items = await storage.ReadAsync<StoreItem>(dict.Keys.ToArray());
            var item = items.FirstOrDefault().Value;

            Assert.Single(items);
            Assert.NotNull(item);
            Assert.NotNull(item.ETag);
            Assert.Equal(StoreItemSample.City, item.City);
            Assert.Equal(StoreItemSample.MessageList, item.MessageList);
        }

        protected async Task UpdateStoreItemTest(IStorage storage)
        {
            var dict = new Dictionary<string, object>
            {
                { "updateItem", StoreItemSample },
            };

            await storage.WriteAsync(dict);

            var items = await storage.ReadAsync(dict.Keys.ToArray());
            var created = items.FirstOrDefault().Value as StoreItem;

            // Update store item
            var list = created.MessageList.ToList();
            list.Add("new message");
            created.MessageList = list.ToArray();

            await storage.WriteAsync(items);

            var updatedItems = await storage.ReadAsync(dict.Keys.ToArray());
            var updated = updatedItems.FirstOrDefault().Value as StoreItem;

            Assert.NotEqual(created.ETag, updated.ETag);
            Assert.Equal(3, updated.MessageList.Length);
            Assert.Equal(created.MessageList, updated.MessageList);
        }

        protected async Task DeleteStoreItemTest(IStorage storage)
        {
            var dict = new Dictionary<string, object>
            {
                { "deleteItem", StoreItemSample },
            };

            await storage.WriteAsync(dict);

            var items = await storage.ReadAsync<StoreItem>(dict.Keys.ToArray());
            var item = items.FirstOrDefault().Value;

            // Delete store item
            await storage.DeleteAsync(dict.Keys.ToArray());

            var deleted = await storage.ReadAsync(dict.Keys.ToArray());

            Assert.NotNull(item);
            Assert.Empty(deleted);
        }

        protected async Task DeleteUnknownStoreItemTest(IStorage storage)
        {
            await storage.DeleteAsync(new[] { "unknown" });
        }

        //protected async Task BatchCreateObjectTest(IStorage storage, long minimumExtraBytes = 0)
        //{
        //    string[] stringArray = null;

        //    if (minimumExtraBytes > 0)
        //    {
        //        // chunks of maximum string size to fill the extra bytes request
        //        var extraStringCount = (int)(minimumExtraBytes / int.MaxValue);
        //        stringArray = Enumerable.Range(0, extraStringCount).Select(i => new string('X', int.MaxValue / 2)).ToArray();

        //        // Append the remaining string size
        //        stringArray = stringArray.Append(new string('X', (int)(minimumExtraBytes % int.MaxValue) / 2)).ToArray();
        //    }

        //    var storeItemsList = new List<Dictionary<string, object>>(new[]
        //        {
        //        new Dictionary<string, object> { ["createPoco"] = new PocoItem() { Id = "1", Count = 0, ExtraBytes = stringArray } },
        //        new Dictionary<string, object> { ["createPoco"] = new PocoItem() { Id = "1", Count = 1, ExtraBytes = stringArray } },
        //        new Dictionary<string, object> { ["createPoco"] = new PocoItem() { Id = "1", Count = 2, ExtraBytes = stringArray } },
        //        });

        //    // TODO: this code as a generic test doesn't make much sense - for now just eliminating the custom exception
        //    // Writing large objects in parallel might raise an InvalidOperationException
        //    try
        //    {
        //        await Task.WhenAll(
        //            storeItemsList.Select(storeItems =>
        //                Task.Run(async () => await storage.WriteAsync(storeItems))));
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsType<InvalidOperationException>(ex);
        //    }

        //    var readStoreItems = new Dictionary<string, object>(await storage.ReadAsync(new[] { "createPoco" }));
        //    Assert.IsType<PocoItem>(readStoreItems["createPoco"]);
        //    var createPoco = readStoreItems["createPoco"] as PocoItem;
        //    Assert.Equal("1", createPoco.Id);
        //}

        //protected async Task StatePersistsThroughMultiTurn(IStorage storage)
        //{
        //    var userState = new UserState(storage);
        //    var testProperty = userState.CreateProperty<TestPocoState>("test");
        //    var adapter = new TestAdapter()
        //        .Use(new AutoSaveStateMiddleware(userState));

        //    await new TestFlow(
        //        adapter,
        //        async (context, cancellationToken) =>
        //        {
        //            var state = await testProperty.GetAsync(context, () => new TestPocoState());
        //            Assert.NotNull(state);
        //            switch (context.Activity.Text)
        //            {
        //                case "set value":
        //                    state.Value = "test";
        //                    await context.SendActivityAsync("value saved");
        //                    break;
        //                case "get value":
        //                    await context.SendActivityAsync(state.Value);
        //                    break;
        //            }
        //        })
        //        .Test("set value", "value saved")
        //        .Test("get value", "test")
        //        .StartTestAsync();
        //}
    }
}
