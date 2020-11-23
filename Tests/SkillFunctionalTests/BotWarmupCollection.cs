using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

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
