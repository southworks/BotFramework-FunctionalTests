using Xunit.Abstractions;

namespace SkillFunctionalTests
{
    /// <summary>
    /// Set up Bot Warmup collection so tests will wait for deployed bot to warm up.
    /// </summary>
    public class BotWarmupFixture
    {
        public BotWarmupFixture()
        {
            ITestOutputHelper outputHelper = null;
            OAuthSkillTest ost = new OAuthSkillTest(outputHelper);

            // OAuthSkillTest.ShouldSignIn() waits for the bot to warm up.
            var result = ost.ShouldSignIn();
            result.Wait();
        }
    }
}
