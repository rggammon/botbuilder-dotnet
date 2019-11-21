﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using CustomMiddleware;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SimpleChildBot
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync($"Skill Error, it looks like something went wrong.\r\n{exception}");
            };

            // Register a couple of dummy middleware instances for testing.
            Use(new DummyMiddleware(">> child-middleware"));
        }
    }
}
