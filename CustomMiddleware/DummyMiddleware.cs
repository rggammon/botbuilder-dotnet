﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace CustomMiddleware
{
    /// <summary>
    /// A dummy middleware to test skills behavior with middleware.
    /// </summary>
    public class DummyMiddleware : IMiddleware
    {
        private readonly string _label;

        public DummyMiddleware(string label)
        {
            _label = label;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            var activity = turnContext.Activity;
            var message = $"{_label}.OnTurnAsync: {Routing(activity)} {activity.Type} {turnContext.Activity.Text}";
            Console.WriteLine(message);

            // Register outgoing handler.
            turnContext.OnSendActivities(OutgoingHandler);

            // Continue processing messages.
            await next(cancellationToken);
        }

        private static string Routing(Activity activity) => $"[{activity.From.Name ?? activity.From.Id}-{activity.Recipient.Name ?? activity.Recipient.Id}]";

        private async Task<ResourceResponse[]> OutgoingHandler(ITurnContext turnContext, List<Activity> activities, Func<Task<ResourceResponse[]>> next)
        {
            // PVA requirements:
            // how do I get the BotId and the SkillId here? 
            // How do I know if this outgoing request is coming from a skill or the host?
            var message = $"{_label}.OnSendActivitiesAsync(leading) [# {string.Join(" # ", activities.Select(a => $"{a.Type} {a.Text}"))} #]";
            Console.WriteLine(message);

            var responses = await next();

            message = $"{_label}.OnSendActivitiesAsync(trailing): {Routing(activities[0])} resource IDs = ({string.Join(", ", responses.Select(r => r.Id))}).";
            Console.WriteLine(message);

            return responses;
        }
    }
}
