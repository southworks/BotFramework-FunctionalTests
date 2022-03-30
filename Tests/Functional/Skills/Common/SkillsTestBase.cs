// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using SkillFunctionalTests.Common;
using Xunit.Abstractions;

namespace SkillFunctionalTests.Skills.Common
{
    public class SkillsTestBase : ScriptTestBase
    {
        public SkillsTestBase(ITestOutputHelper output)
            : base(output)
        {
        }

        public static List<string> DeliveryModes { get; } = new List<string>
        {
            Microsoft.Bot.Schema.DeliveryModes.Normal,
            Microsoft.Bot.Schema.DeliveryModes.ExpectReplies
        };

        public static List<HostBot> SimpleHostBots { get; } = new List<HostBot>
        {
            HostBot.SimpleHostBotComposerDotNet,
            HostBot.SimpleHostBotDotNet,
            HostBot.SimpleHostBotJS,
            HostBot.SimpleHostBotPython,
        };

        public static List<HostBot> WaterfallHostBots { get; } = new List<HostBot>
        {
            HostBot.ComposerHostBotDotNet,
            HostBot.WaterfallHostBotDotNet,
            HostBot.WaterfallHostBotJS,
            HostBot.WaterfallHostBotPython,
        };

        public static List<SkillBot> EchoSkillBots { get; } = new List<SkillBot>
        {
            SkillBot.EchoSkillBotComposerDotNet,
            SkillBot.EchoSkillBotDotNet,
            SkillBot.EchoSkillBotDotNetV3,
            SkillBot.EchoSkillBotJS,
            SkillBot.EchoSkillBotJSV3,
            SkillBot.EchoSkillBotPython
        };

        public static List<SkillBot> WaterfallSkillBots { get; } = new List<SkillBot>
        {
            SkillBot.WaterfallSkillBotDotNet,
            SkillBot.WaterfallSkillBotJS,
            SkillBot.WaterfallSkillBotPython,
            SkillBot.ComposerSkillBotDotNet
        };

        public static IEnumerable<object[]> BuildTestCases(
            List<string> scripts,
            List<HostBot> hosts,
            List<SkillBot> skills,
            List<string> channels = default,
            List<string> deliveryModes = default,
            Func<SkillsTestCase, bool> exclude = default)
        {
            var cases = from channel in channels ?? Channels
                        from deliveryMode in deliveryModes ?? DeliveryModes
                        from bot in hosts
                        from skill in skills
                        from script in scripts
                        select new SkillsTestCase
                        {
                            Channel = channel,
                            DeliveryMode = deliveryMode,
                            Bot = bot,
                            Skill = skill,
                            Script = script
                        };

            return cases
                .Where(e => exclude == null || !exclude(e))
                .Select(e => new object[] { new TestCaseDataObject<SkillsTestCase>(e) });
        }
    }
}
