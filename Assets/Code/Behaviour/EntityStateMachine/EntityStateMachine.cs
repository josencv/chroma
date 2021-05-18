using Chroma.Behaviour.EntityStateMachine.States;
using Chroma.Components.Movement;
using Chroma.Game.Commands;
using Chroma.Infrastructure.FSM;
using UnityEngine;
using Zenject;

namespace Chroma.Behaviour.EntityStateMachine
{
    class EntityStateMachine : MonoBehaviour
    {
        private const string MovementSpeedFieldName = "movementSpeed";

        private CommandableStateMachine stateMachine;
        private Movable movable;
        private InputCommandIssuer commandIssuer;
        public CommandableStateMachine StateMachine { get { return stateMachine; } }

        [Inject]
        private void Inject(Movable movable, InputCommandIssuer commandIssuer)
        {
            this.movable = movable;
            this.commandIssuer = commandIssuer;
        }

        private void Awake()
        {
            stateMachine = BuildStateMachine();
            stateMachine.Start();
            commandIssuer.CommandIssued += stateMachine.ProcessCommand;
        }

        private void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        // TODO: use scriptable object or json to read state machine data instead of manually building it
        private CommandableStateMachine BuildStateMachine()
        {
            var stateMachine = new CommandableStateMachine();
            stateMachine.RegisterFloatField(MovementSpeedFieldName, 0);

            var idleState = new IdleState("idle", movable);
            var movingState = new MovingState("moving", movable);
            var idleMovingTransition = new StateTransition(idleState, movingState);
            idleMovingTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Float, ConditionOperator.Greater, MovementSpeedFieldName, 0));
            idleState.AddTransition(idleMovingTransition);
            var movingIdleTransition = new StateTransition(movingState, idleState);
            movingIdleTransition.AddCondition(new TransitionCondition(StateMachineFieldType.Float, ConditionOperator.Equal, MovementSpeedFieldName, 0));
            movingState.AddTransition(movingIdleTransition);

            stateMachine.SetEntryPoint(idleState);

            return stateMachine;
        }
    }
}
