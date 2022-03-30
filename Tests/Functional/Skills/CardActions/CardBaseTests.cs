// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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

namespace SkillFunctionalTests.Skills.CardActions
{
    [Trait("TestCategory", "CardActions")]
    public abstract class CardBaseTests : SkillsTestBase
    {
        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Skills/CardActions/TestScripts";

        public CardBaseTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static Func<SkillsTestCase, bool> Exclude(Func<SkillsTestCase, bool> exclude = default)
        {
            // TODO: Enable after fixing differences in the Cards scripts.
            return (SkillsTestCase test) => test.Skill == SkillBot.ComposerSkillBotDotNet || (exclude != null && exclude(test));
        }

        public static IEnumerable<object[]> TestCases(
            List<string> scripts = default,
            Func<SkillsTestCase, bool> exclude = default) => BuildTestCases(scripts: scripts, hosts: WaterfallHostBots, skills: WaterfallSkillBots, exclude: Exclude(exclude));

        public virtual async Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData)
        {
            var testCase = testData.GetObject();
            Logger.LogInformation(JsonConvert.SerializeObject(testCase, Formatting.Indented));

            var options = TestClientOptions[testCase.Bot];
            var runner = new XUnitTestRunner(new TestClientFactory(testCase.Channel, options, Logger).GetTestClient(), TestRequestTimeout, ThinkTime, Logger);

            var testParams = new Dictionary<string, string>
            {
                { "DeliveryMode", testCase.DeliveryMode },
                { "TargetSkill", testCase.Skill.ToString() }
            };

            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, "WaterfallGreeting.json"), testParams);
            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, testCase.Script), testParams);
        }
    }
}
