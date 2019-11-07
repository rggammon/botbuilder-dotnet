﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace SimpleRootBot.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;

        public BotController(BotAdapter adapter, IBot bot)
        {
            _adapter = (BotFrameworkHttpAdapter)adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            try
            {
                // Entering parent
                var authToken = Request.Headers["Authorization"].ToString();

                // Delegate the processing of the HTTP POST to the adapter.
                // The adapter will invoke the bot.
                await _adapter.ProcessAsync(Request, Response, _bot);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}