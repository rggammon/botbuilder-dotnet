﻿// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions
{
    /// <summary>
    /// Actions triggered when a MessageDeleteActivity is received.
    /// </summary>
    public class OnMessageDeleteActivity : OnActivity
    {
        [JsonProperty("$type")]
        public const string DeclarativeType = "Microsoft.OnMessageDeleteActivity";

        [JsonConstructor]
        public OnMessageDeleteActivity(List<Dialog> actions = null, string constraint = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(type: ActivityTypes.MessageDelete, actions: actions, constraint: constraint, callerPath: callerPath, callerLine: callerLine)
        {
        }
    }
}
