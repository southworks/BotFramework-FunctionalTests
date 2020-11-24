using System;
using System.IO;
using System.Threading;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TranscriptTestRunner;
using TranscriptTestRunner.XUnit;
using Xunit;
using Xunit.Abstractions;
using ActivityTypes = Microsoft.Bot.Connector.DirectLine.ActivityTypes;

namespace SkillFunctionalTests
{
    /// <summary>
    /// This xUnit <see cref="ICollectionFixture{TFixture}"/> lets tests wait for the deployed bot to warm up.
    /// </summary>
    public class BotWarmupFixture
    {
        public BotWarmupFixture()
        {
            var result = WarmupManualTest();
            result.Wait();
        }

        private async System.Threading.Tasks.Task WarmupManualTest()
        {
            Console.WriteLine("Starting bot warmup.");

            var runner = new XUnitTestRunner(new TestClientFactory(ClientType.DirectLine).GetTestClient(), null);

            int retries = 6;                        // Defines the allowed warmup period.
            int timeBetweenRetriesMs = 30 * 1000;

            while (retries >= 0)
            {
                try
                {
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
                        throw;
                    }
                }

                Console.WriteLine("Bot warmup completed.");
                break;
            }
        }
    }
}
