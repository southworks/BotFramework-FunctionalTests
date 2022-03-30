// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const {
  ActivityHandler,
  ActivityTypes,
  EndOfConversationCodes
} = require('botbuilder');

class EchoBot extends ActivityHandler {
  constructor () {
    super();

    // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
    this.onMessage(async (context, next) => {
      const activityText = context.activity.text.toLowerCase();
      if (activityText === 'end' || activityText === 'stop') {
        await context.sendActivity('Ending conversation from the skill...');
        await context.sendActivity({
          type: ActivityTypes.EndOfConversation,
          code: EndOfConversationCodes.CompletedSuccessfully
        });
      } else {
        await context.sendActivity(`Echo: ${context.activity.text}`);
        await context.sendActivity(
          'Say "end" or "stop" and I\'ll end the conversation and back to the parent.'
        );
      }

      // By calling next() you ensure that the next BotHandler is run.
      await next();
    });

    this.onUnrecognizedActivityType(async (_context, next) => {
      // This will be called if the root bot is ending the conversation.  Sending additional messages should be
      // avoided as the conversation may have been deleted.
      // Perform cleanup of resources if needed.

      // By calling next() you ensure that the next BotHandler is run.
      await next();
    });

    this.onMembersAdded(async (turnContext, next) => {
      const text = 'Welcome to the echo skill bot. \n\nThis is a skill, you will need to call it from another bot to use it.';

      for (const member of turnContext.activity.membersAdded) {
        if (member.id !== turnContext.activity.recipient.id) {
          await turnContext.sendActivity({
            type: ActivityTypes.Message,
            text,
            speak: text.replace('\n\n', '')
          });
        }
      }
    });
  }
}

module.exports = { EchoBot };
