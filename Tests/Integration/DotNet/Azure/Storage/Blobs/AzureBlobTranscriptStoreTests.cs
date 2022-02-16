// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Azure.Storage.Blobs
{
    [Trait("TestCategory", "Deprecated")]
    public class AzureBlobTranscriptStoreTests : BlobsTranscriptStoreBaseTests, IClassFixture<AzureBlobTranscriptStoreFixture>
    {
        public AzureBlobTranscriptStoreTests(ITestOutputHelper outputHandler, AzureBlobTranscriptStoreFixture blobFixture)
            : base(outputHandler)
        {
            UseStorages(blobFixture.Storages);
        }
    }
}
