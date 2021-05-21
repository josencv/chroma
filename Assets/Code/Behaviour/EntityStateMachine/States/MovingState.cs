using Chroma.Components.Movement;
using Chroma.Game.Commands;

namespace Chroma.Behaviour.EntityStateMachine.States
{
    class MovingState : CommandableState
    {
        private Movable movable;

        public MovingState(string name, Movable movable) : base(name)
        {
            this.movable = movable;
        }

        public override void Enter() {}

        public override void Exit() { }

        public override void Interrupt() { }

        public override void ProcessCommand(Command command, CommandArgs args)
        {
            if(command == Command.Move)
            {
                movable.Move(args.Vector2Value);
            }
        }

        public override void Update(float deltaTime) { }
    }
}
