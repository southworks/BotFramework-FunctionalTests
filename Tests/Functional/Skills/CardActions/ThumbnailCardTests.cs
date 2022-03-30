// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using SkillFunctionalTests.Common;
using SkillFunctionalTests.Skills.Common;
using Xunit;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Skills.CardActions
{
    [Trait("TestCategory", "CardActions")]
    public class ThumbnailCardTests : CardBaseTests
    {
        private static readonly List<string> Scripts = new List<string>
        {
            "Thumbnail.json"
        };

        public ThumbnailCardTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> TestCases() => TestCases(scripts: Scripts);

        [Theory]
        [MemberData(nameof(TestCases))]
        public override Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData) => RunTestCases(testData);
    }
}
