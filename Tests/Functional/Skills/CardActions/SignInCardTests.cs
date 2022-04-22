﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Tests.Functional.Common;
using Microsoft.Bot.Builder.Tests.Functional.Skills.Common;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Bot.Builder.Tests.Functional.Skills.CardActions
{
    [Trait("TestCategory", "CardActions")]
    public class SignInCardTests : CardBaseTests
    {
        private static readonly List<string> Scripts = new List<string>
        {
            "SignIn.json"
        };

        public SignInCardTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> TestCases() => TestCases(scripts: Scripts);

        [Theory]
        [MemberData(nameof(TestCases))]
        public override Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData) => base.RunTestCases(testData);
    }
}
