# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from typing import List
from botbuilder.core import ActivityHandler, MessageFactory, TurnContext
from botbuilder.schema import Activity, ActivityTypes, ChannelAccount, EndOfConversationCodes

class EchoBot(ActivityHandler):
    async def on_message_activity(self, turn_context: TurnContext):
        if "end" in turn_context.activity.text or "stop" in turn_context.activity.text:
            # Send End of conversation at the end.
            await turn_context.send_activity(
                MessageFactory.text("Ending conversation from the skill...")
            )

            end_of_conversation = Activity(type=ActivityTypes.end_of_conversation)
            end_of_conversation.code = EndOfConversationCodes.completed_successfully
            await turn_context.send_activity(end_of_conversation)
        else:
            await turn_context.send_activity(
                MessageFactory.text(f"Echo: {turn_context.activity.text}")
            )
            await turn_context.send_activity(
                MessageFactory.text(
                    f'Say "end" or "stop" and I\'ll end the conversation and back to the parent.'
                )
            )

    async def on_end_of_conversation_activity(self, turn_context: TurnContext):
        # This will be called if the host bot is ending the conversation. Sending additional messages should be
        # avoided as the conversation may have been deleted.
        # Perform cleanup of resources if needed.
        pass

    async def on_members_added_activity(
        self, members_added: List[ChannelAccount], turn_context: TurnContext
    ):
        text = (
            "Welcome to the waterfall skill bot. \n\n"
            "This is a skill, you will need to call it from another bot to use it."
        )

        for member in members_added:
            if member.id != turn_context.activity.recipient.id:
                await turn_context.send_activity(
                    Activity(
                        type=ActivityTypes.message,
                        text=text,
                        speak=text.replace("\n\n", ""),
                    )
                )
