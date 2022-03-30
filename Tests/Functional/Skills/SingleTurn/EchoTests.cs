// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkillFunctionalTests.Common;
using SkillFunctionalTests.Skills.Common;
using TranscriptTestRunner;
using TranscriptTestRunner.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Skills.SingleTurn
{
    [Trait("TestCategory", "SingleTurn")]
    public class EchoTests : SkillsTestBase
    {
        private static readonly List<string> Scripts = new List<string>
        {
            "EchoMultiSkill.json"
        };

        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Skills/SingleTurn/TestScripts";

        public EchoTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static bool Exclude(SkillsTestCase test)
        {
            // This local function is used to exclude ExpectReplies test cases for v3 bots
            if (test.DeliveryMode == Microsoft.Bot.Schema.DeliveryModes.ExpectReplies)
            {
                // Note: ExpectReplies is not supported by DotNetV3 and JSV3 skills.
                return test.Skill == SkillBot.EchoSkillBotDotNetV3 || test.Skill == SkillBot.EchoSkillBotJSV3;
            }

            return false;
        }

        public static IEnumerable<object[]> TestCases() => BuildTestCases(scripts: Scripts, hosts: SimpleHostBots, skills: EchoSkillBots, exclude: Exclude);

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData)
        {
            var testCase = testData.GetObject();
            Logger.LogInformation(JsonConvert.SerializeObject(testCase, Formatting.Indented));

            var options = TestClientOptions[testCase.Bot];

            var client = new TestClientFactory(testCase.Channel, options, Logger).GetTestClient();
            var runner = new XUnitTestRunner(client, TestRequestTimeout, ThinkTime, Logger);

            var testParams = new Dictionary<string, string>
            {
                { "DeliveryMode", testCase.DeliveryMode },
                { "TargetSkill", testCase.Skill.ToString() }
            };

            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, testCase.Script), testParams);
        }
    }
}
