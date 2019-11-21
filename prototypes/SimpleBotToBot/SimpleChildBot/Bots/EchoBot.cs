﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace SimpleChildBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("end") || turnContext.Activity.Text.Contains("stop"))
            {
                // Send End of conversation at the end.
                await turnContext.SendActivityAsync(MessageFactory.Text($"ending conversation from the skill..."), cancellationToken);

                // Unknown = "unknown";
                // CompletedSuccessfully = "completedSuccessfully";
                // UserCancelled = "userCancelled";
                // BotTimedOut = "botTimedOut";
                // BotIssuedInvalidMessage = "botIssuedInvalidMessage";
                // ChannelFailed = "channelFailed";
                var endOfConversation = Activity.CreateEndOfConversationActivity();
                endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
                var response = await turnContext.SendActivityAsync(endOfConversation, cancellationToken);
            }
            else
            {
                var responses = await turnContext.SendActivitiesAsync(
                    new Activity[]
                    {
                        MessageFactory.Text($"Echo : {turnContext.Activity.Text}"),
                        MessageFactory.Text($"Say \"end\" or \"stop\" and I'll end the conversation and back to the parent.'")
                    },
                    cancellationToken);
            }
        }
    }
}
