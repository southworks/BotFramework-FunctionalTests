// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Xunit;

namespace IntegrationTests.Azure.Storage.Queues
{
    public class AzureQueueBaseFixture : ConfigurationFixture, IAsyncLifetime
    {
        public AzureQueueBaseFixture()
        {
            ConnectionString = Configuration["Azure:Storage:ConnectionString"];
        }

        public string ConnectionString { get; private set; }

        protected bool IsRunning { get; private set; }

        public async Task InitializeAsync()
        {
            IsRunning = await IsServiceRunning();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task<bool> IsServiceRunning()
        {
            try
            {
                using var cancellation = new CancellationTokenSource(Timeout);
                await new QueueServiceClient(ConnectionString).GetPropertiesAsync(cancellation.Token);
                return true;
            }
            catch (TaskCanceledException ex)
            {
                const string message = "StorageAccount: Unable to connect to the 'Queues' endpoint.";
                throw new TaskCanceledException(message, ex);
            }
        }
    }
}
