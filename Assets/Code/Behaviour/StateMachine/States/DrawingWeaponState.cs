using Chroma.Behaviour.Attack;
using Chroma.Behaviour.Movement;
using Chroma.Game.Commands;

namespace Chroma.Behaviour.StateMachine.States
{
    class DrawingWeaponState : CommandableState
    {
        private Movable movable;
        private WeaponController weaponController;

        public DrawingWeaponState(string name, Movable movable, WeaponController weaponController) : base(name)
        {
            this.movable = movable;
            this.weaponController = weaponController;
        }

        public override void ProcessCommand(Command command, CommandArgs args)
        {
            if(command == Command.Move)
            {
                movable.Move(args.Vector2Value);
            }
        }

        public override void Enter()
        {
            base.Enter();
            weaponController.DrawWeapon();
        }
    }
}
