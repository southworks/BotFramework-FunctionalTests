// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillFunctionalTests.Bot;
using SkillFunctionalTests.Configuration;

namespace FunctionalTests
{
    [TestClass]
    [TestCategory("FunctionalTests")]
    public class SimpleHostBotToEchoSkillTest
    {
        [TestMethod]
        public async Task Host_WhenRequested_ShouldRedirectToSkill()
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            var testBot = new TestBotClient(new EnvironmentBotTestConfiguration());

            await testBot.StartConversation(cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.SendMessageAsync("Hi", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.AssertReplyAsync("Me no nothin", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.SendMessageAsync("skill", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.AssertReplyAsync("Echo: skill", cancellationTokenSource.Token);
        }

        [TestMethod]
        public async Task Host_WhenSkillEnds_HostReceivesEndOfConversation()
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            var testBot = new TestBotClient(new EnvironmentBotTestConfiguration());

            await testBot.StartConversation(cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.SendMessageAsync("Hi", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.AssertReplyAsync("Me no nothin", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.SendMessageAsync("skill", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.AssertReplyAsync("Echo: skill", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.SendMessageAsync("end", cancellationTokenSource.Token);
            Thread.Sleep(200);
            await testBot.AssertReplyAsync("Received endOfConversation", cancellationTokenSource.Token);
        }
    }
}
