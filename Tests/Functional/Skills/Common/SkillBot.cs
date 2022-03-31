// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SkillFunctionalTests.Skills.Common
{
    public enum SkillBot
    {
        /// <summary>
        /// Echo skill implemented using Composer and the DotNet runtime.
        /// </summary>
        EchoSkillBotComposerDotNet,

        /// <summary>
        /// Echo skill implemented using using DotNet 3.1.
        /// </summary>
        EchoSkillBotDotNet,

        /// <summary>
        /// Echo v3 skill implemented using using DotNet 3.1.
        /// </summary>
        EchoSkillBotDotNetV3,

        /// <summary>
        /// Echo skill implemented using using JS.
        /// </summary>
        EchoSkillBotJS,

        /// <summary>
        /// Echo v3 skill implemented using using JS.
        /// </summary>
        EchoSkillBotJSV3,

        /// <summary>
        /// Echo skill implemented using using Python.
        /// </summary>
        EchoSkillBotPython,

        /// <summary>
        /// Waterfall skill implemented using using DotNet 3.1.
        /// </summary>
        WaterfallSkillBotDotNet,

        /// <summary>
        /// Waterfall skill implemented using using JS.
        /// </summary>
        WaterfallSkillBotJS,

        /// <summary>
        /// Waterfall skill implemented using using Python.
        /// </summary>
        WaterfallSkillBotPython,

        /// <summary>
        /// Waterfall host implemented using Composer and the DotNet runtime.
        /// </summary>
        ComposerSkillBotDotNet,
    }
}
