using Chroma.Behaviour.Attack;
using Chroma.Behaviour.Movement;
using Chroma.Behaviour.Skills;
using Chroma.Behaviour.StateMachine.States;
using Chroma.Game.Commands;
using Chroma.Infrastructure.FSM;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.StateMachine
{
    class EntityStateMachine : MonoBehaviour
    {
        public const string MovementSpeedFieldName = "movementSpeed";
        public const string ActionEndedFieldName = "actionEnded";

        private Absorber absorber;
        private CommandableStateMachine stateMachine;
        private InputCommandIssuer commandIssuer;
        private Movable movable;
        private WeaponController weaponController;
        public CommandableStateMachine StateMachine { get { return stateMachine; } }

        [Inject]
        private void Inject(Absorber absorber, Movable movable, InputCommandIssuer commandIssuer, WeaponController weaponController)
        {
            this.absorber = absorber;
            this.commandIssuer = commandIssuer;
            this.movable = movable;
            this.weaponController = weaponController;
        }

        private void Awake()
        {
            stateMachine = BuildStateMachine();
            stateMachine.Start();
            commandIssuer.CommandIssued += stateMachine.ProcessCommand;
        }

        private void Update()
        {
            //stateMachine.Update(Time.deltaTime);
        }

        // TODO: use scriptable object or json to read state machine data instead of manually building it
        private CommandableStateMachine BuildStateMachine()
        {
            var stateMachine = new CommandableStateMachine();
            stateMachine.RegisterFloatField(MovementSpeedFieldName, 0);
            stateMachine.RegisterTriggerField(ActionEndedFieldName);

            var idleState = new IdleState("Idle", absorber, movable, weaponController);
            var movingState = new MovingState("Moving", movable, weaponController);
            var drawingWeaponState = new DrawingWeaponState("Drawing Weapon", movable, weaponController);
            var battleIdleState = new BattleIdleState("Battle Idle", absorber, movable, weaponController);
            var battleMovingState = new BattleMovingState("Battle Moving", movable, weaponController);
            var sheathingWeaponState = new SheathingWeaponState("Sheathing Weapon", movable, weaponController);
            var absorbingState = new AbsorbingState("Absorbing", absorber, movable);

            var actionEndedCondition = new TransitionCondition(StateMachineFieldType.Trigger, ConditionOperator.None, ActionEndedFieldName);

            var idleToMovingTransition = new StateTransition(idleState, movingState);
            idleToMovingTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Float, ConditionOperator.Greater, MovementSpeedFieldName, 0));
            idleState.AddTransition(idleToMovingTransition);

            var movingToIdleTransition = new StateTransition(movingState, idleState);
            movingToIdleTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Float, ConditionOperator.Equal, MovementSpeedFieldName, 0));
            movingState.AddTransition(movingToIdleTransition);

            var idleToDrawingWeaponTransition = new StateTransition(idleState, drawingWeaponState);
            idleToDrawingWeaponTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Int, ConditionOperator.Equal, CommandableStateMachine.CommandFieldName, (int) Command.AttackConfirm));
            idleState.AddTransition(idleToDrawingWeaponTransition);

            var idleToAbsorbingTransition = new StateTransition(idleState, absorbingState);
            idleToAbsorbingTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Int, ConditionOperator.Equal, CommandableStateMachine.CommandFieldName, (int)Command.AbsorbStart));
            idleState.AddTransition(idleToAbsorbingTransition);

            var movingToDrawingWeaponTransition = new StateTransition(movingState, drawingWeaponState);
            movingToDrawingWeaponTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Int, ConditionOperator.Equal, CommandableStateMachine.CommandFieldName, (int)Command.AttackConfirm));
            movingState.AddTransition(movingToDrawingWeaponTransition);

            var drawingWeaponToBattleIdleTransition = new StateTransition(drawingWeaponState, battleIdleState);
            drawingWeaponToBattleIdleTransition.AddCondition(actionEndedCondition);
            drawingWeaponState.AddTransition(drawingWeaponToBattleIdleTransition);

            var battleIdleToBattleMovingTransition = new StateTransition(battleIdleState, battleMovingState);
            battleIdleToBattleMovingTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Float, ConditionOperator.Greater, MovementSpeedFieldName, 0));
            battleIdleState.AddTransition(battleIdleToBattleMovingTransition);

            var battleMovingToBattleIdleTransition = new StateTransition(battleMovingState, battleIdleState);
            battleMovingToBattleIdleTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Float, ConditionOperator.Equal, MovementSpeedFieldName, 0));
            battleMovingState.AddTransition(battleMovingToBattleIdleTransition);

            var battleIdleToSheathingWeaponTransition = new StateTransition(battleIdleState, sheathingWeaponState);
            battleIdleToSheathingWeaponTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Int, ConditionOperator.Equal, CommandableStateMachine.CommandFieldName, (int)Command.Sheathe));
            battleIdleState.AddTransition(battleIdleToSheathingWeaponTransition);

            var battleMovingToSheathingWeaponTransition = new StateTransition(battleMovingState, sheathingWeaponState);
            battleMovingToSheathingWeaponTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Int, ConditionOperator.Equal, CommandableStateMachine.CommandFieldName, (int)Command.Sheathe));
            battleMovingState.AddTransition(battleMovingToSheathingWeaponTransition);

            var sheathingWeaponToIdleTransition = new StateTransition(sheathingWeaponState, idleState);
            sheathingWeaponToIdleTransition.AddCondition(actionEndedCondition);
            sheathingWeaponState.AddTransition(sheathingWeaponToIdleTransition);

            stateMachine.SetEntryPoint(idleState);

            return stateMachine;
        }
    }
}
