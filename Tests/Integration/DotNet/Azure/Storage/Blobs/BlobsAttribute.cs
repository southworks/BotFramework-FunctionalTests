// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace IntegrationTests.Azure.Storage.Blobs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BlobsAttribute : Attribute
    {
        public BlobsAttribute(string containerId = "BlobsContainer")
        {
            ContainerId = containerId;
        }

        public string ContainerId { get; private set; }
    }
}
