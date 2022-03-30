// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using SkillFunctionalTests.Common;
using SkillFunctionalTests.Standalone.Common;

namespace SkillFunctionalTests.Standalone.Authentication
{
    public class AuthTestCase : TestCase<Bot>
    {
        public MicrosoftAppType AppType { get; set; }

        public override string ToString()
        {
            return $"{Script}, {Bot}, {AppType}, {Channel}";
        }
    }
}
