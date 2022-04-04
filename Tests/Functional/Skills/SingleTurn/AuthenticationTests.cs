// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkillFunctionalTests.Common;
using SkillFunctionalTests.Skills.Common;
using Xunit;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Skills.SingleTurn
{
    [Trait("TestCategory", "Authentication")]
    public class AuthenticationTests : EchoBaseTests
    {
        private static readonly List<HostBot> Hosts = SimpleHostMSIBots.Union(SimpleHostSTBots).ToList();

        private static readonly List<SkillBot> Skills = EchoSkillMSIBots.Union(EchoSkillSTBots).ToList();

        public AuthenticationTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public static IEnumerable<object[]> TestCases() => BuildTestCases(scripts: Scripts, hosts: Hosts, skills: Skills);

        [Theory]
        [MemberData(nameof(TestCases))]
        public override Task RunTestCases(TestCaseDataObject<SkillsTestCase> testData) => base.RunTestCases(testData);
    }
}
