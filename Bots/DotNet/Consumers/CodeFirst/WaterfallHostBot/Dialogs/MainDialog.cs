// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.BotFrameworkFunctionalTests.WaterfallHostBot.Dialogs.Sso;
using Microsoft.BotFrameworkFunctionalTests.WaterfallHostBot.Skills;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.BotFrameworkFunctionalTests.WaterfallHostBot.Dialogs
{
    /// <summary>
    /// The main dialog for this bot. It uses a <see cref="SkillDialog"/> to call skills.
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        // State property key that stores the active skill (used in AdapterWithErrorHandler to terminate the skills on error).
        public static readonly string ActiveSkillPropertyName = $"{typeof(MainDialog).FullName}.ActiveSkillProperty";

        private const string SsoDialogPrefix = "Sso";
        private readonly IStatePropertyAccessor<BotFrameworkSkill> _activeSkillProperty;
        private readonly string _deliveryMode = $"{typeof(MainDialog).FullName}.DeliveryMode";
        private readonly string _selectedSkillKey = $"{typeof(MainDialog).FullName}.SelectedSkillKey";
        private readonly SkillsConfiguration _skillsConfig;
        private readonly IConfiguration _configuration;

        // Dependency injection uses this constructor to instantiate MainDialog.
        public MainDialog(ConversationState conversationState, SkillConversationIdFactoryBase conversationIdFactory, SkillHttpClient skillClient, SkillsConfiguration skillsConfig, IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var botId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;

            _skillsConfig = skillsConfig ?? throw new ArgumentNullException(nameof(skillsConfig));

            if (skillClient == null)
            {
                throw new ArgumentNullException(nameof(skillClient));
            }

            if (conversationState == null)
            {
                throw new ArgumentNullException(nameof(conversationState));
            }

            // Create state property to track the active skill.
            _activeSkillProperty = conversationState.CreateProperty<BotFrameworkSkill>(ActiveSkillPropertyName);

            // Add ChoicePrompt to render available delivery modes.
            AddDialog(new ChoicePrompt("FeedbackPrompt"));

            AddDialog(new ChoicePrompt("CheckAnswer"));

            // Add main waterfall dialog for this bot.
            var waterfallSteps = new WaterfallStep[]
            {
                SelectDeliveryModeStepAsync,
                CheckAnswer
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            
            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SelectDeliveryModeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageText = "give feedback";
            const string retryMessageText = "invalid";
            var choices = new List<Choice>
            {
                new Choice("😀"),
                new Choice("⭐")
            };
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text(retryMessageText, retryMessageText, InputHints.ExpectingInput),
                Choices = choices
            };

            // Prompt the user to select a delivery mode.
            return await stepContext.PromptAsync("FeedbackPrompt", options, cancellationToken);
        }

        private async Task<DialogTurnResult> CheckAnswer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var response = ((FoundChoice)stepContext.Result).Value;
            var messageText = $"Is the {response} correct?";
            const string retryMessageText = "invalid";
            var choices = new List<Choice>
            {
                new Choice("yes"),
                new Choice("no")
            };
            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput),
                RetryPrompt = MessageFactory.Text(retryMessageText, retryMessageText, InputHints.ExpectingInput),
                Choices = choices
            };

            return await stepContext.PromptAsync("CheckAnswer", options, cancellationToken);
        }
    }
}
