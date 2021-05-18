using System;
using System.Collections.Generic;
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
        }
    }
}
