using Xunit;

namespace SkillFunctionalTests
{
    [CollectionDefinition("Wait for bot warmup")]
    public class BotWarmupCollection : ICollectionFixture<BotWarmupFixture>
    {
        // Sets up BotWarmupFixture so tests will wait for deployed bot to warm up.
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
