﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.Bot.Builder.Dialogs.Flow
{
    /// <summary>
    /// Execute set of commands in sequence
    /// </summary>
    /// <remarks>
    /// The behavior of executing multiple actions that try to manipulate the DialogStack is not defined.  
    /// </remarks>
    public class CommandSet : IFlowCommand
    {
        public CommandSet(List<IFlowCommand> actions = null)
        {
            if (actions != null)
                this.Commands = actions;
        }

        public List<IFlowCommand> Commands { get; set; } = new List<IFlowCommand>();

        public async Task<DialogTurnResult> Execute(DialogContext dialogContext, object options, DialogTurnResult result, CancellationToken cancellationToken)
        {
            foreach (var action in Commands)
            {
                result = await action.Execute(dialogContext, options, result, cancellationToken);
            }
            return result;
        }
    }
}