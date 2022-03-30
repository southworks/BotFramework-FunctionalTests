// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillFunctionalTests.Common;
using SkillFunctionalTests.Skills.Common;
using SkillFunctionalTests.Standalone.Authentication;
using SkillFunctionalTests.Standalone.Common;
using TranscriptTestRunner.TestClients;
using Xunit.Abstractions;

namespace SkillFunctionalTests
{
    public class ScriptTestBase
    {
        public ScriptTestBase(ITestOutputHelper output)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddConsole()
                    .AddDebug()
                    .AddFile(Directory.GetCurrentDirectory() + @"/Logs/Log.json", isJson: true)
                    .AddXunit(output);
            });

            Logger = loggerFactory.CreateLogger<ScriptTestBase>();

            TestRequestTimeout = int.Parse(configuration["TestRequestTimeout"]);
            TestClientOptions = configuration.GetSection("HostBotClientOptions").Get<Dictionary<HostBot, DirectLineTestClientOptions>>();
            TestAuthClientOptions = configuration.GetSection("AuthBotClientOptions").Get<Dictionary<Bot, Dictionary<MicrosoftAppType, DirectLineTestClientOptions>>>();
            ThinkTime = int.Parse(configuration["ThinkTime"]);
        }

        public static List<string> Channels { get; } = new List<string>
        {
            Microsoft.Bot.Connector.Channels.Directline
        };

        public Dictionary<HostBot, DirectLineTestClientOptions> TestClientOptions { get; }

        public Dictionary<Bot, Dictionary<MicrosoftAppType, DirectLineTestClientOptions>> TestAuthClientOptions { get; }

        public ILogger Logger { get; }

        public int TestRequestTimeout { get; }

        public int ThinkTime { get; }
    }
}
