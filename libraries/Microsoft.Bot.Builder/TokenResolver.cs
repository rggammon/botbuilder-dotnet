﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder
{
    internal static class TokenResolver
    {
        private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(1);   // Poll for token every 1 second.

        public static void CheckForOAuthCards(BotFrameworkAdapter adapter, ILogger logger, ITurnContext turnContext, Activity activity, CancellationToken cancellationToken)
        {
            if (activity == null || activity.Attachments == null)
            {
                return;
            }

            foreach (var attachment in activity.Attachments.Where(a => a.ContentType == OAuthCard.ContentType))
            {
                OAuthCard oauthCard = attachment.Content as OAuthCard;
                if (oauthCard != null)
                {
                    if (string.IsNullOrWhiteSpace(oauthCard.ConnectionName))
                    {
                        throw new InvalidOperationException("The OAuthPrompt's ConnectionName property is missing a value.");
                    }

                    // Poll as a background task
                    Task.Run(() => PollForTokenAsync(adapter, logger, turnContext, activity, oauthCard.ConnectionName, cancellationToken))
                        .ContinueWith(t =>
                        {
                            if (t.Exception != null)
                            {
                                logger.LogError(t.Exception.InnerException ?? t.Exception, "PollForTokenAsync threw an exception", oauthCard.ConnectionName);
                            }
                        });
                }
            }
        }

        private static async Task PollForTokenAsync(BotFrameworkAdapter adapter, ILogger logger, ITurnContext turnContext, Activity activity, string connectionName, CancellationToken cancellationToken)
        {
            TokenResponse tokenResponse = null;
            bool shouldEndPolling = false;
            var pollingTimeout = TurnStateConstants.OAuthLoginTimeoutValue;
            var pollingRequestsInterval = PollingInterval;
            var loginTimeout = turnContext.TurnState.Get<object>(TurnStateConstants.OAuthLoginTimeoutKey);
            bool sentToken = false;

            // Override login timeout with value set from the OAuthPrompt or by the developer
            if (turnContext.TurnState.ContainsKey(TurnStateConstants.OAuthLoginTimeoutKey))
            {
                pollingTimeout = (TimeSpan)turnContext.TurnState.Get<object>(TurnStateConstants.OAuthLoginTimeoutKey);
            }

            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < pollingTimeout && !shouldEndPolling)
            {
                tokenResponse = await adapter.GetUserTokenAsync(turnContext, connectionName, null, cancellationToken).ConfigureAwait(false);

                if (tokenResponse != null)
                {
                    // This can be used to short-circuit the polling loop.
                    if (tokenResponse.Properties != null)
                    {
                        JToken tokenPollingSettingsToken = null;
                        TokenPollingSettings tokenPollingSettings = null;
                        tokenResponse.Properties.TryGetValue(TurnStateConstants.TokenPollingSettingsKey, out tokenPollingSettingsToken);

                        if (tokenPollingSettingsToken != null)
                        {
                            tokenPollingSettings = tokenPollingSettingsToken.ToObject<TokenPollingSettings>();

                            if (tokenPollingSettings != null)
                            {
                                logger.LogInformation($"PollForTokenAsync received new polling settings: timeout={tokenPollingSettings.Timeout}, interval={tokenPollingSettings.Interval}", tokenPollingSettings);
                                shouldEndPolling = tokenPollingSettings.Timeout <= 0 ? true : shouldEndPolling; // Timeout now and stop polling
                                pollingRequestsInterval = tokenPollingSettings.Interval > 0 ? TimeSpan.FromMilliseconds(tokenPollingSettings.Interval) : pollingRequestsInterval; // Only overrides if it is set.
                            }
                        }
                    }

                    // once there is a token, send it to the bot and stop polling
                    if (tokenResponse.Token != null)
                    {
                        var tokenResponseActivityEvent = CreateTokenResponse(turnContext.Activity.GetConversationReference(), tokenResponse.Token, connectionName);
                        var identity = turnContext.TurnState.Get<IIdentity>(BotFrameworkAdapter.BotIdentityKey) as ClaimsIdentity;
                        var callback = turnContext.TurnState.Get<BotCallbackHandler>();
                        await adapter.ProcessActivityAsync(identity, tokenResponseActivityEvent, callback, cancellationToken).ConfigureAwait(false);
                        shouldEndPolling = true;
                        sentToken = true;

                        logger.LogInformation("PollForTokenAsync completed with a token", turnContext.Activity);
                    }
                }

                if (!shouldEndPolling)
                {
                    await Task.Delay(pollingRequestsInterval).ConfigureAwait(false);
                }
            }

            if (!sentToken)
            {
                logger.LogInformation("PollForTokenAsync completed without receiving a token", turnContext.Activity);
            }

            stopwatch.Stop();
        }

        private static Activity CreateTokenResponse(ConversationReference relatesTo, string token, string connectionName)
        {
            var tokenResponse = Activity.CreateEventActivity() as Activity;

            // IActivity properties
            tokenResponse.Id = Guid.NewGuid().ToString();
            tokenResponse.Timestamp = DateTime.UtcNow;
            tokenResponse.From = relatesTo.User;
            tokenResponse.Recipient = relatesTo.Bot;
            tokenResponse.ReplyToId = relatesTo.ActivityId;
            tokenResponse.ServiceUrl = relatesTo.ServiceUrl;
            tokenResponse.ChannelId = relatesTo.ChannelId;
            tokenResponse.Conversation = relatesTo.Conversation;
            tokenResponse.Attachments = new List<Attachment>().ToArray();
            tokenResponse.Entities = new List<Entity>().ToArray();

            // IEventActivity properties
            tokenResponse.Name = "tokens/response";
            tokenResponse.RelatesTo = relatesTo;
            tokenResponse.Value = new TokenResponse()
            {
                Token = token,
                ConnectionName = connectionName,
            };

            return tokenResponse;
        }
    }
}
