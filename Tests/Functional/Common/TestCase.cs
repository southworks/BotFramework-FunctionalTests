// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SkillFunctionalTests.Skills.Common
{
    public class TestCase<T>
    {
        public string Description { get; set; }

        public string ChannelId { get; set; }

        public T HostBot { get; set; }

        public string Script { get; set; }
    }
}
