using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Chroma.Game.Commands
{
    class InputCommandIssuer
    {
        public event Action<Command, CommandArgs> CommandIssued;

        public InputCommandIssuer(PlayerInput playerInput)
        {
            playerInput.onActionTriggered += MapInputToCommand;
        }

        private void MapInputToCommand(CallbackContext context)
        {
            string actionName = context.action.name;

            if (actionName == "Move")
            {
                CommandIssued?.Invoke(Command.Move, new CommandArgs(context.ReadValue<Vector2>()));
            }
            else if (actionName == "MoveCamera")
            {
                CommandIssued?.Invoke(Command.MoveCamera, new CommandArgs(context.ReadValue<Vector2>()));
            }
            else if(actionName == "AttackConfirm")
            {
                CommandIssued?.Invoke(Command.AttackConfirm, new CommandArgs());
            }
            else if(actionName == "Sheathe")
            {
                CommandIssued?.Invoke(Command.Sheathe, new CommandArgs());
            }
            else if(actionName == "AbsorbStart")
            {
                CommandIssued?.Invoke(Command.AbsorbStart, new CommandArgs());
            }
            else if(actionName == "AbsorbRelease")
            {
                CommandIssued?.Invoke(Command.AbsorbRelease, new CommandArgs());
            }
        }
    }
}
