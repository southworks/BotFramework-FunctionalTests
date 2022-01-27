// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;

namespace IntegrationTests.Azure
{
    public class PocoStoreItem : IStoreItem
    {
        public string ETag { get; set; }

        public string Id { get; set; }

        public int Count { get; set; }
    }
}
