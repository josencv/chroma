using Chroma.Behaviour.Attack;
using Chroma.Behaviour.Movement;
using Chroma.Game.Commands;
using Zenject;

namespace Chroma.Behaviour.StateMachine.States
{
    class BattleMovingState : CommandableState
    {
        private Movable movable;
        private WeaponController weaponController;

        public BattleMovingState(string name, [InjectOptional] Movable movable, [InjectOptional] WeaponController weaponController) : base(name)
        {
            this.movable = movable;
            this.weaponController = weaponController;
        }

        public override void ProcessCommand(Command command, CommandArgs args)
        {
            if(command == Command.Move)
            {
                movable?.Move(args.Vector2Value);
            }
            else if(command == Command.Sheathe)
            {
                weaponController?.SheatheWeapon();
            }
        }
    }
}
