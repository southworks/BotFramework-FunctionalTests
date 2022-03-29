// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkillFunctionalTests;
using SkillFunctionalTests.Skills.Common;
using SkillFunctionalTests.Standalone.Authentication;
using SkillFunctionalTests.Standalone.Common;
using TranscriptTestRunner;
using TranscriptTestRunner.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace SkillsFunctionalTests.Standalone.Authentication
{
    [Trait("TestCategory", "Authentication")]
    public class AuthenticationTests : ScriptTestBase
    {
        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Standalone/Authentication/TestScripts";

        public AuthenticationTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> TestCases()
        {
            var channelIds = new List<string> { Channels.Directline };

            var bots = new List<Bot>
            {
                Bot.EchoBotDotNet,
                
                //Bot.EchoBotJS
            };

            var auth = new List<MicrosoftAppType>
            {
                MicrosoftAppType.MultiTenant,
                
                //MicrosoftAppType.SingleTenant,
                //MicrosoftAppType.UserAssignedMsi
            };

            var scripts = new List<string> { "Echo.json" };

            var testCaseBuilder = new TestCaseBuilder();

            var testCases = testCaseBuilder.BuildTestCases(channelIds, bots, auth, scripts);
            foreach (var testCase in testCases)
            {
                yield return testCase;
            }
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTestCases(TestCaseDataObject testData)
        {
            var authTestCase = testData.GetObject<AuthTestCase>();
            Logger.LogInformation(JsonConvert.SerializeObject(authTestCase, Formatting.Indented));

            var options = TestAuthClientOptions[authTestCase.Bot][authTestCase.AppType];

            var runner = new XUnitTestRunner(new TestClientFactory(authTestCase.ChannelId, options, Logger).GetTestClient(), TestRequestTimeout, ThinkTime, Logger);

            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, authTestCase.Script));
        }
    }
}
