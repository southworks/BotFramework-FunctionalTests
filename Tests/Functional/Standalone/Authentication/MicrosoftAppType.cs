// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SkillFunctionalTests.Standalone.Authentication
{
    public enum MicrosoftAppType
    {
        /// <summary>
        /// MultiTenant app which uses botframework.com tenant to acquire tokens.
        /// </summary>
        MultiTenant,

        /// <summary>
        /// SingleTenant app which uses the bot's host tenant to acquire tokens.
        /// </summary>
        SingleTenant,

        /// <summary>
        /// App with a user assigned Managed Identity (MSI), which will be used as the AppId for token acquisition.
        /// </summary>
        UserAssignedMsi
    }
}
