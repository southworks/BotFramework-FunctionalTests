## Requirements

- [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21) latest version.
- [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=npm) equal or grater than 3.15.0 version.
  > **Note (02-18-2022):** Visual Studio 2022 is installing Azurite `3.14.1`, which makes some tests to not work properly on expired Queues.

## Integration tests

| Library                                                                                   | Tests for                                                                                                                                                  | Requirement                                                | Configuration    |
| ----------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- | ---------------- |
| [Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure) | Cosmos                                                                                                                                                     | [Azure Cosmos DB Emulator](#requirements) running          | [link](#cosmos)  |
| [Microsoft.Bot.Builder.Azure](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure) | [Blobs](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure.Blobs) <br> [Queues](https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure.Queues) | [Azurite](#requirements) Blobs and Queues services running | [link](#storage) |

## Configuration

The test project can be configured using `Environment variables`, and either the [appsettings.json](appsettings.json) or `appsettings.Development.json` files.

> **Notes:**
>
> - **Order of importance:** `Environment variables` => `appsettings.Development.json` => `appsettings.json`.
> - **Environment variables syntax**: `e.g. Azure:Cosmos:ServiceEndpoint=https://localhost:8081`.

### Emulators

The test project is already configured out of the box to use the supported Emulators.

### Cosmos

Cosmos tests require the `ServiceEndpoint` and `AuthKey` properties to authenticate the requests when using the service.<br>
For more information on how to gather the connection values, use the following links:

- [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#authenticate-requests)
- [Azure Cosmos DB account](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/manage-with-cli#list-connection-strings)
  > **Note:** `AccountEndpoint` is equal to `ServiceEndpoint` and `AccountKey` is equal to `AuthKey`.

### Storage

Storage tests require the `ConnectionString` property to authenticate the requests when using the service.<br>
For more information on how to gather the connection values, use the following links:

- [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string#configure-a-connection-string-for-azurite)
- [Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string#configure-a-connection-string-for-an-azure-storage-account)
