// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SkillFunctionalTests.Skills.Common
{
    public enum HostBot
    {
        /// <summary>
        /// Waterfall host implemented using Composer and the DotNet runtime.
        /// </summary>
        ComposerHostBotDotNet,

        /// <summary>
        /// Simple host implemented using Composer and the DotNet runtime.
        /// </summary>
        SimpleHostBotComposerDotNet,

        /// <summary>
        /// Simple host implemented using DotNet 3.1.
        /// </summary>
        SimpleHostBotDotNet,

        /// <summary>
        /// Simple host implemented using JS.
        /// </summary>
        SimpleHostBotJS,

        /// <summary>
        /// Simple host implemented using Python.
        /// </summary>
        SimpleHostBotPython,

        /// <summary>
        /// Host implemented using DotNet and waterfall dialogs.
        /// </summary>
        WaterfallHostBotDotNet,

        /// <summary>
        /// Host implemented using JS and waterfall dialogs.
        /// </summary>
        WaterfallHostBotJS,

        /// <summary>
        /// Host implemented using Python and waterfall dialogs.
        /// </summary>
        WaterfallHostBotPython,
    }
}
