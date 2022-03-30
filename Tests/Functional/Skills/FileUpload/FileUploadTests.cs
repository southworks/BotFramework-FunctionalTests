// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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

namespace SkillFunctionalTests.Skills.FileUpload
{
    [Trait("TestCategory", "FileUpload")]
    public class FileUploadTests : SkillsTestBase
    {
        private static readonly List<string> Scripts = new List<string>
        {
            "FileUpload1.json"
        };

        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Skills/FileUpload/TestScripts";

        public FileUploadTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> TestCases() => BuildTestCases(scripts: Scripts, hosts: WaterfallHostBots, skills: WaterfallSkillBots);

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData)
        {
            var testGuid = Guid.NewGuid().ToString();
            var fileName = $"TestFile-{testGuid}.txt";
            var testCase = testData.GetObject();
            Logger.LogInformation(JsonConvert.SerializeObject(testCase, Formatting.Indented));

            var options = TestClientOptions[testCase.Bot];
            var runner = new XUnitTestRunner(new TestClientFactory(testCase.Channel, options, Logger).GetTestClient(), TestRequestTimeout, ThinkTime, Logger);

            // Execute the first part of the conversation.
            var testParams = new Dictionary<string, string>
            {
                { "DeliveryMode", testCase.DeliveryMode },
                { "TargetSkill", testCase.Skill.ToString() },
                { "FileName", fileName },
                { "TestGuid", testGuid }
            };

            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, testCase.Script), testParams);

            // Create a new file to upload.
            await using var stream = File.Create(Directory.GetCurrentDirectory() + $"/Skills/FileUpload/{fileName}");
            await using var writer = new StreamWriter(stream);
            await writer.WriteLineAsync($"GUID:{testGuid}");
            writer.Close();

            // Upload file.
            await using var file = File.OpenRead(Directory.GetCurrentDirectory() + $"/Skills/FileUpload/{fileName}");
            await runner.UploadAsync(file);

            // Execute the rest of the conversation.
            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, "FileUpload2.json"), testParams);
        }
    }
}
