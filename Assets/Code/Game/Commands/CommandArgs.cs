using UnityEngine;

namespace Chroma.Game.Commands
{
    struct CommandArgs
    {
        public Vector2 Vector2Value { get; }

        public CommandArgs(Vector2 vector2Value)
        {
            Vector2Value = vector2Value;
        }
    }
}
