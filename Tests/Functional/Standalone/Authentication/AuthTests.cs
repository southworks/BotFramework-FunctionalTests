// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkillFunctionalTests.Common;
using TranscriptTestRunner;
using TranscriptTestRunner.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Standalone.Authentication
{
    [Trait("TestCategory", "Authentication")]
    public class AuthTests : AuthTestBase
    {
        private static readonly List<string> Scripts = new List<string>
        {
            "Echo.json"
        };

        private readonly string _testScriptsFolder = Directory.GetCurrentDirectory() + @"/Standalone/Authentication/TestScripts";

        public AuthTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> TestCases() => BuildTestCases(scripts: Scripts);

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTestCases(TestCaseDataObject<AuthTestCase> testData)
        {
            var authTestCase = testData.GetObject();
            Logger.LogInformation(JsonConvert.SerializeObject(authTestCase, Formatting.Indented));

            var options = TestAuthClientOptions[authTestCase.Bot][authTestCase.AppType];

            var runner = new XUnitTestRunner(new TestClientFactory(authTestCase.Channel, options, Logger).GetTestClient(), TestRequestTimeout, ThinkTime, Logger);

            await runner.RunTestAsync(Path.Combine(_testScriptsFolder, authTestCase.Script));
        }
    }
}
