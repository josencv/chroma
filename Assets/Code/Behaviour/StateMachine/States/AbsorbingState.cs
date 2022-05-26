using Chroma.Behaviour.Movement;
using Chroma.Behaviour.Skills;
using Chroma.Game.Commands;
using Zenject;

namespace Chroma.Behaviour.StateMachine.States
{
    class AbsorbingState : CommandableState
    {
        private Absorber absorber;
        private Movable movable;

        public AbsorbingState(string name, Absorber absorber, [InjectOptional] Movable movable) : base(name)
        {
            this.absorber = absorber;
            this.movable = movable;
        }

        public override void ProcessCommand(Command command, CommandArgs args)
        {
            if(command == Command.Move)
            {
                movable?.Move(args.Vector2Value);
            }
            else if(command == Command.AbsorbRelease)
            {
                // TODO: de-hardcode color
                absorber.ExecuteAbsobption();
            }
        }
    }
}
