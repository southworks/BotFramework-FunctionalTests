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
    /// Set up Bot Warmup collection so tests will wait for deployed bot to warm up.
    /// </summary>
    public class BotWarmupFixture
    {
        //private readonly ILogger<SimpleHostBotToEchoSkillTest> _logger;
        //private readonly ITestOutputHelper output = null;

        public BotWarmupFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            //NullLoggerFactory loggerFactory;

            //    LoggerFactory.null.Create(builder =>
            //{
            //    builder
            //        .AddConfiguration(configuration)
            //        .AddConsole()
            //        .AddDebug()
            //        .AddFile(Directory.GetCurrentDirectory() + @"/Logs/Log.json", isJson: true)
            //        .AddXunit(output);
            //});

            // = (ILogger<SimpleHostBotToEchoSkillTest>)loggerFactory.CreateLogger<OAuthSkillTest>();

            //OAuthSkillTest ost = new OAuthSkillTest(_output);

            // OAuthSkillTest.ShouldSignIn() waits for the bot to warm up.
            //var result = ost.ShouldSignIn();
            var result = WarmupManualTest();
            result.Wait();
        }

        public async System.Threading.Tasks.Task WarmupManualTest()
        {
            var runner = new XUnitTestRunner(new TestClientFactory(ClientType.DirectLine).GetTestClient(), null);

            int retries = 3;        // This gives a chance for the newly deployed bot to warm up.
            int waitMs = 60 * 1000;

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
                catch
                {
                    if (retries > 0)
                    {
                        retries--;

                        //_logger.LogInformation($"Retrying the test. Retries = {retries}");
                        Console.WriteLine($"Retrying the test. Retries = {retries}");
                        Thread.Sleep(waitMs);
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }

                break;
            }
        }
    }
}
