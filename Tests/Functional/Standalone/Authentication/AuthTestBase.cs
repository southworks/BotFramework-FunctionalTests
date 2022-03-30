using System;
using System.Collections.Generic;
using System.Linq;
using SkillFunctionalTests.Common;
using SkillFunctionalTests.Standalone.Common;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Standalone.Authentication
{
    public class AuthTestBase : ScriptTestBase
    {
        public AuthTestBase(ITestOutputHelper output)
            : base(output)
        {
        }

        public static List<MicrosoftAppType> AppTypes { get; } = new List<MicrosoftAppType>
        {
            MicrosoftAppType.MultiTenant,
            MicrosoftAppType.SingleTenant,
            MicrosoftAppType.UserAssignedMsi
        };

        public static List<Bot> Bots { get; } = new List<Bot>
        {
            Bot.EchoBotDotNet,
            Bot.EchoBotJS,
        };

        public static IEnumerable<object[]> BuildTestCases(
            List<string> scripts,
            List<Bot> bots = default,
            List<string> channels = default,
            List<MicrosoftAppType> appTypes = default,
            Func<AuthTestCase, bool> exclude = default)
        {
            var cases = from channel in channels ?? Channels
                        from appType in appTypes ?? AppTypes
                        from bot in bots ?? Bots
                        from script in scripts
                        select new AuthTestCase
                        {
                            Channel = channel,
                            AppType = appType,
                            Bot = bot,
                            Script = script
                        };

            return cases
                .Where(e => exclude == null || !exclude(e))
                .Select(e => new object[] { new TestCaseDataObject<AuthTestCase>(e) });
        }
    }
}
