// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TranscriptTestRunner;
using TranscriptTestRunner.XUnit;
using Xunit;
using ActivityTypes = Microsoft.Bot.Connector.DirectLine.ActivityTypes;

namespace SkillFunctionalTests
{
    /// <summary>
    /// This xUnit class fixture lets tests wait for the deployed bot to warm up <see cref="ICollectionFixture{TFixture}"/>.
    /// </summary>
    public class BotWarmupFixture
    {
        private readonly string _transcriptsFolder = Directory.GetCurrentDirectory() + @"/SourceTranscripts";

        public BotWarmupFixture()
        {
            Console.WriteLine("Debug: Start 1st warmup");
            var result = WarmupManualTest("1");
            result.Wait();

            Console.WriteLine("Debug: 1st warmup completed. ");

            //var result2 = WarmupManualTest("2");
            //result2.Wait();
            //Console.WriteLine("Debug: 2nd warmup completed");
        }

        private async Task WarmupManualTest(string logMessage)
        {
            Console.WriteLine($"Starting bot warmup {logMessage}.");

            int retries = 6;                        // Defines the allowed warmup period.
            int timeBetweenRetriesMs = 30 * 1000;

            while (retries >= 0)
            {
                try
                {
                    var runner = new XUnitTestRunner(new TestClientFactory(ClientType.DirectLine).GetTestClient(), null);

                    await runner.SendActivityAsync(new Activity(ActivityTypes.ConversationUpdate));

                    await runner.AssertReplyAsync(activity =>
                    {
                        Assert.Equal(ActivityTypes.Message, activity.Type);
                        Assert.Equal("Hello and welcome!", activity.Text);
                    });
                }
                catch (Exception e)
                {
                    if (retries > 0)
                    {
                        retries--;

                        Console.WriteLine(e.Message);
                        Console.WriteLine($"Waiting for warmup. Retries = {retries}");
                        Thread.Sleep(timeBetweenRetriesMs);
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Bot warmup failed.");
                        throw new Exception($"Bot warmup failed: {e.Message}");
                    }
                }

                Console.WriteLine("Bot warmup completed.");
                break;
            }
        }

        [Fact(Timeout = 2000)]
        private async Task WarmupShouldSignIn()
        {
            Console.WriteLine("Starting bot warmup.");

            var runner = new XUnitTestRunner(new TestClientFactory(ClientType.DirectLine).GetTestClient(), null);
            var signInUrl = string.Empty;

            int retries = 4;                        // Defines the allowed warmup period.
            int timeBetweenRetriesMs = 30 * 1000;

            while (retries >= 0)
            {
                try
                {
                    // Execute the first part of the conversation.
                    await runner.RunTestAsync(Path.Combine(_transcriptsFolder, "ShouldSignIn1.transcript"));

                    // Obtain the signIn url.
                    await runner.AssertReplyAsync(activity =>
                    {
                        Assert.Equal(ActivityTypes.Message, activity.Type);
                        Assert.True(activity.Attachments.Count > 0);

                        var card = JsonConvert.DeserializeObject<SigninCard>(JsonConvert.SerializeObject(activity.Attachments.FirstOrDefault().Content));
                        signInUrl = card.Buttons[0].Value?.ToString();

                        Assert.False(string.IsNullOrEmpty(signInUrl));
                    });

                    // Execute the SignIn.
                    // await runner.ClientSignInAsync(signInUrl);

                    // Execute the rest of the conversation.
                    // await runner.RunTestAsync(Path.Combine(_transcriptsFolder, "ShouldSignIn2.transcript"));
                }
                catch (Exception e)
                {
                    if (retries > 0)
                    {
                        retries--;

                        Console.WriteLine(e.Message);
                        Console.WriteLine($"Waiting for warmup. Retries = {retries}");
                        Thread.Sleep(timeBetweenRetriesMs);
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Bot warmup failed.");
                        throw new Exception($"Bot warmup failed: {e.Message}");
                    }
                }

                Console.WriteLine("Bot warmup completed.");
                break;
            }
        }
    }
}
