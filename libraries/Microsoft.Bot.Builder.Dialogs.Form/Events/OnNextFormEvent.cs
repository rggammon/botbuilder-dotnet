﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Form.Events
{
    /// <summary>
    /// Triggered when a form needs to clarify which value is meant.
    /// </summary>
    public class OnNextFormEvent : OnDialogEvent
    {
        [JsonConstructor]
        public OnNextFormEvent(List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(
                @event: FormEvents.NextFormEvent,
                actions: actions,
                condition: condition,
                callerPath: callerPath,
                callerLine: callerLine)
        {
        }
    }
}
