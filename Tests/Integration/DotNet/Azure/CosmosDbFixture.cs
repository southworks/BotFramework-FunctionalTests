// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace IntegrationTests.Azure
{
    public class CosmosDbFixture : IAsyncLifetime
    {
        // Endpoint and Authkey for the CosmosDB Emulator running locally.
        // See https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#authenticate-requests for details on the well known key being used.
        private const string EmulatorServiceEndpoint = "https://localhost:8081";
        private const string EmulatorAuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public string ServiceEndpoint { get; private set; }

        public string AuthKey { get; private set; }

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            ServiceEndpoint = configuration["Azure:CosmosDB:ServiceEndpoint"];
            AuthKey = configuration["Azure:CosmosDB:AuthKey"];
            
            await IsServiceRunning();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task<bool> IsServiceRunning()
        {
            using var client = new DocumentClient(new Uri(ServiceEndpoint), AuthKey);
            try
            {
                await client.OpenAsync();
                return true;
            }
            catch (HttpRequestException ex)
            {
                var message = $"CosmosDB: Unable to connect to the '{ServiceEndpoint}' endpoint.";
                throw new HttpRequestException(message, ex);
            }
        }
    }
}
