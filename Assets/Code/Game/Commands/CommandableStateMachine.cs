﻿using Chroma.Infrastructure.FSM;

namespace Chroma.Game.Commands
{
    class CommandableStateMachine : StateMachine
    {
        public const string CommandFieldName = "command";

        public CommandableStateMachine(CommandableState entryPoint) : base(entryPoint) 
        {
            InitializeRequiredFields();
        }

        public CommandableStateMachine() : base(new CommandableEmptyState())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            RegisterIntField(CommandFieldName, (int)Command.None);
        }

        public void ProcessCommand(Command command, CommandArgs args)
        {
            SetIntField(CommandFieldName, (int)command);

            if(CurrentState != null && CurrentState is CommandableState)
            {
                ((CommandableState)CurrentState).ProcessCommand(command, args);
            }
        }
    }
}