// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using SkillFunctionalTests.Standalone.Authentication;
using SkillFunctionalTests.Standalone.Common;

namespace SkillFunctionalTests.Skills.Common
{
    public class AuthTestCase : TestCase<Bot>
    {
        public MicrosoftAppType AppType { get; set; }

        public Bot Bot { get; set; }
    }
}
