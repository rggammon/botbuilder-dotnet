﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.LanguageGeneration.Templates;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Form.Actions
{
    public class Ask : SendActivity
    {
        [JsonConstructor]
        public Ask(
            string text = null,
            List<string> expectedSlots = null,
            [CallerFilePath] string callerPath = "",
            [CallerLineNumber] int callerLine = 0)
        : base(text, callerPath, callerLine)
        {
            this.RegisterSourceLocation(callerPath, callerLine);
            this.Activity = new ActivityTemplate(text ?? string.Empty);
            this.ExpectedSlots = expectedSlots;
        }

        /// <summary>
        /// Gets or sets slots expected to be filled by response.
        /// </summary>
        /// <value>
        /// Slots expected to be filled by response.
        /// </value>
        public List<string> ExpectedSlots { get; set; } = new List<string>();

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            dc.State.SetValue("dialog.expectedSlots", ExpectedSlots);
            var result = await base.BeginDialogAsync(dc, options, cancellationToken).ConfigureAwait(false);
            result.Status = DialogTurnStatus.CompleteAndWait;
            return result;
        }
    }
}