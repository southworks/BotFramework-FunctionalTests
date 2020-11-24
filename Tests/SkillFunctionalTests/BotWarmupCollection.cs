using Xunit;

namespace SkillFunctionalTests
{
    /// <summary>
    /// This Collection attribute helps tests wait for the deployed bot to warm up.
    /// </summary>
    [CollectionDefinition("Wait for bot warmup")]
    public class BotWarmupCollection : ICollectionFixture<BotWarmupFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
