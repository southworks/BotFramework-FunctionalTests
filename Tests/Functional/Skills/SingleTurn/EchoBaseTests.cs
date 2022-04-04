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
using Xunit.Abstractions;

namespace SkillFunctionalTests.Skills.SingleTurn
{
    public class EchoBaseTests : SkillsTestBase
    {
        protected static readonly List<string> Scripts = new List<string>
        {
            "EchoMultiSkill.json"
        };

        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Skills/SingleTurn/TestScripts";

        public EchoBaseTests(ITestOutputHelper output)
            : base(output)
        {
        }

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

            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, testCase.Script), testParams);
        }
    }
}
