// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;

namespace IntegrationTests.Azure
{
    [CollectionDefinition("CosmosDb")]
    public class CosmosDbCollectionDefinition : ICollectionFixture<CosmosDbFixture>
    {
    }
}
