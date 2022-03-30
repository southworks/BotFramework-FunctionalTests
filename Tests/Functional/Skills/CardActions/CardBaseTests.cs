// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
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
    public class CardBaseTests : SkillsTestBase
    {
        //private static readonly List<string> Scripts = new List<string>
        //{
        //    "BotAction.json",
        //    "TaskModule.json",
        //    "SubmitAction.json",
        //    "Hero.json",
        //    "Thumbnail.json",
        //    "Receipt.json",
        //    "SignIn.json",
        //    "Carousel.json",
        //    "List.json",
        //    "O365.json",
        //    "Animation.json",
        //    "Audio.json",
        //    "Video.json"
        //};

        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Skills/CardActions/TestScripts";

        public CardBaseTests(ITestOutputHelper output)
            : base(output)
        {
        }

        //public static bool ShouldExclude(SkillsTestCase testCase)
        //{
        //    // This local function is used to exclude ExpectReplies, O365 and WaterfallSkillBotPython test cases
        //    if (testCase.Script == "O365.json")
        //    {
        //        // BUG: O365 fails with ExpectReplies for WaterfallSkillBotPython (remove when https://github.com/microsoft/BotFramework-FunctionalTests/issues/328 is fixed).
        //        return testCase.Skill == SkillBot.WaterfallSkillBotPython && testCase.DeliveryMode == Microsoft.Bot.Schema.DeliveryModes.ExpectReplies;
        //    }

        //    return false;
        //}

        public static IEnumerable<object[]> TestCases(List<string> scripts = default) => BuildTestCases(scripts: scripts, hosts: WaterfallHostBots, skills: WaterfallSkillBots);

        public async Task RunTestCases2(TestCaseDataObject<SkillsTestCase> testData)
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
