/* eslint-disable jsdoc/require-returns */
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, InputHints, MessageFactory } = require('botbuilder');
const {
    ChoicePrompt,
    ChoiceFactory,
    ComponentDialog,
    DialogSet,
    WaterfallDialog,
    DialogTurnStatus,
    ListStyle,
} = require('botbuilder-dialogs');
const { RootBot } = require('../bots/rootBot');
const { SsoDialog } = require('./sso/ssoDialog');
const { TangentDialog } = require('./tangentDialog');

const MAIN_DIALOG = 'MainDialog';
const FEEDBACK_PROMPT = 'FeedbackPrompt';
const CONFIRMATION_PROMPT = 'ConfirmationPrompt';
const TANGENT_DIALOG = 'TangentDialog';
const WATERFALL_DIALOG = 'WaterfallDialog';
const SSO_DIALOG_PREFIX = 'Sso';

class MainDialog extends ComponentDialog {
    /**
     * @param {import('botbuilder').ConversationState} conversationState
     * @param {import('../skillsConfiguration').SkillsConfiguration} skillsConfig
     * @param {import('botbuilder').SkillHttpClient} skillClient
     * @param {import('../skillConversationIdFactory').SkillConversationIdFactory} conversationIdFactory
     */
    constructor(conversationState, skillsConfig, skillClient, conversationIdFactory) {
        super(MAIN_DIALOG);

        const botId = process.env.MicrosoftAppId;

        if (!conversationState) throw new Error("[MainDialog]: Missing parameter 'conversationState' is required");
        if (!skillsConfig) throw new Error("[MainDialog]: Missing parameter 'skillsConfig' is required");
        if (!skillClient) throw new Error("[MainDialog]: Missing parameter 'skillClient' is required");
        if (!conversationIdFactory)
            throw new Error("[MainDialog]: Missing parameter 'conversationIdFactory' is required");

        this.feedbackProperty = conversationState.createProperty(RootBot.feedbackPropertyName);
        this.activeSkillProperty = conversationState.createProperty(RootBot.ActiveSkillPropertyName);
        this.skillsConfig = skillsConfig;
        this.deliveryMode = '';

        // Register the tangent dialog for testing tangents and resume.
        this.addDialog(new TangentDialog(TANGENT_DIALOG));

        // Add ChoicePrompt to render available delivery modes.
        this.addDialog(new ChoicePrompt(FEEDBACK_PROMPT));

        // Add ChoicePrompt to render available groups of skills.
        this.addDialog(new ChoicePrompt(CONFIRMATION_PROMPT));

        this.addDialog(
            new WaterfallDialog(WATERFALL_DIALOG, [
                this.askFeedbackStep.bind(this),
                this.ConfirmationStep.bind(this),
                this.finalStep.bind(this),
            ])
        );

        this.initialDialogId = WATERFALL_DIALOG;
    }

    async run(turnContext, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    /**
     * @param {import('botbuilder-dialogs').DialogContext} innerDc
     */
    async onContinueDialog(innerDc) {
        const activeSkill = await this.activeSkillProperty.get(innerDc.context, () => null);
        const activity = innerDc.context.activity;
        if (
            activeSkill != null &&
            activity.type === ActivityTypes.Message &&
            activity.text != null &&
            activity.text.toLowerCase() === 'abort'
        ) {
            // Cancel all dialogs when the user says abort.
            // The SkillDialog automatically sends an EndOfConversation message to the skill to let the
            // skill know that it needs to end its current dialogs, too.
            await innerDc.cancelAllDialogs();
            return innerDc.replaceDialog(this.initialDialogId, {
                text: 'Canceled! \n\n What delivery mode would you like to use?',
            });
        }
        return super.onContinueDialog(innerDc);
    }

    async askFeedbackStep(stepContext) {
        const CHOICES = ['⭐', '⭐⭐', '⭐⭐⭐'];

        return await stepContext.prompt(FEEDBACK_PROMPT, {
            choices: ChoiceFactory.toChoices(CHOICES),
            style: ListStyle.suggestedAction,
        });
    }

    async ConfirmationStep(stepContext) {
        this.feedback = stepContext.result.value;

        const messageText = `You gave us a ${this.feedback} rate. Is that correct?`;
        const retryMessageText = 'That was not a valid choice, please select a valid option.';

        // Create the PromptOptions from the skill configuration which contains the list of configured skills.
        return stepContext.prompt(CONFIRMATION_PROMPT, {
            prompt: MessageFactory.text(messageText, messageText, InputHints.ExpectingInput),
            retryPrompt: MessageFactory.text(retryMessageText, retryMessageText, InputHints.ExpectingInput),
            choices: ChoiceFactory.toChoices(['Yes', 'No']),
        });
    }

    async finalStep(stepContext) {
        const confirmation = stepContext.result.value;
        let messageText = '';

        if (confirmation == 'Yes') {
            messageText = 'Thank you for your feedback!';
        } else {
            messageText = 'Thank you for your feedback!';
        }
        await stepContext.context.sendActivity(messageText);

        // Restart dialog
        return stepContext.replaceDialog(this.initialDialogId);
    }
}

module.exports.MainDialog = MainDialog;
