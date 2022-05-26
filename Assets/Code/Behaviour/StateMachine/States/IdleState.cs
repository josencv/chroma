using Chroma.Behaviour.Attack;
using Chroma.Behaviour.Movement;
using Chroma.Behaviour.Skills;
using Chroma.Game.Commands;
using Zenject;

namespace Chroma.Behaviour.StateMachine.States
{
    class IdleState : CommandableState
    {
        private Absorber absorber;
        private Movable movable;
        private WeaponController weaponController;

        public IdleState(
            string name,
            [InjectOptional] Absorber absorber,
            [InjectOptional] Movable movable,
            [InjectOptional] WeaponController weaponController
        ) : base(name)
        {
            this.absorber = absorber;
            this.movable = movable;
            this.weaponController = weaponController;
        }

        public override void ProcessCommand(Command command, CommandArgs args) 
        {
            if(command == Command.Move)
            {
                movable?.Move(args.Vector2Value);
            }
            else if(command == Command.AttackConfirm)
            {
                weaponController?.DrawWeapon();
            }
            else if(command == Command.AbsorbStart)
            {
                absorber?.StartAbsorption();
            }
        }
    }
}
