﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using SkillFunctionalTests.Common;
using SkillFunctionalTests.Skills.Common;
using Xunit;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Skills.SingleTurn
{
    [Trait("TestCategory", "SingleTurn")]
    public class EchoTests : EchoBaseTests
    {
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
        public override Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData) => base.RunTestCases(testData);
    }
}
