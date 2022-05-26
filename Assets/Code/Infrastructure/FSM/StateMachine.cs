using System;
using System.Collections.Generic;

namespace Chroma.Infrastructure.FSM
{
    /// <summary>
    /// A generic state machine, similar to Unity's
    /// </summary>
    class StateMachine
    {
        private Dictionary<string, StateMachineField> Fields { get; }  // The values of the state machine variables. Used to trigger state transitions
        protected List<State> states;
        private readonly State entryPoint;
        public State CurrentState { get; private set; }
        private readonly List<string> triggerNamesToClear;

        /// <summary>
        /// Initializes an instance of the StateMachine class
        /// </summary>
        public StateMachine(State entryPoint)
        {
            this.entryPoint = entryPoint;
            Fields = new Dictionary<string, StateMachineField>();
            triggerNamesToClear = new List<string>();
            states = new List<State>();
        }

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="state"></param>
        public virtual void AddState(State state)
        {
            states.Add(state);
        }

        /// <summary>
        /// Sets the given state as the entry point (or start state) of the state machine.
        /// When the machine state starts, this states will be run first
        /// Note: in the future this might be useful for implementing sub-state machines
        /// </summary>
        /// <param name="state"></param>
        public void SetEntryPoint(State state)
        {
            entryPoint.AddTransition(new StateTransition(entryPoint, state));
        }

        /// <summary>
        /// Starts the state machine.
        /// </summary>
        public virtual void Start()
        {
            if (entryPoint.Transitions.Count == 0) 
            {
                throw new ApplicationException("cannot start state machine without an entry point");
            }

            CurrentState = entryPoint.Transitions[0].To;
            CurrentState.Enter();
        }

        /// <summary>
        /// Update call to be used inside the game update loop
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        public virtual void Update(float deltaTime)
        {
            ClearTriggers();

            if (CurrentState != null)
            {
                CurrentState.Update(deltaTime);
            }
        }

        private void ClearTriggers()
        {
            foreach(string triggerName in triggerNamesToClear)
            {
                Fields[triggerName].Value = 0.0f;
            }

            triggerNamesToClear.Clear();
        }

        /// <summary>
        /// Checks if any transition of the current state meets the requirements to be applied
        /// </summary>
        protected virtual void EvaluateTransitions()
        {
            foreach (StateTransition transition in CurrentState.Transitions)
            {
                if (transition.ShouldApplyTransition(Fields))
                {
                    ApplyTransition(transition);
                    break;
                }
            }
        }

        /// <summary>
        /// Apply a transition of the current state and the state machine current state is changed to the one pointed by the transition
        /// </summary>
        /// <param name="transition"></param>
        protected virtual void ApplyTransition(StateTransition transition)
        {
            CurrentState = transition.To;
            transition.Apply();
        }

        /// <summary>
        /// Registers a boolean field to the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="startingValue">The value of the field when the state machine starts</param>
        public void RegisterBoolField(string fieldName, bool startingValue = false)
        {
            float fvalue = startingValue ? 1 : 0;
            StateMachineField field = new StateMachineField(StateMachineFieldType.Bool, fvalue);
            Fields.Add(fieldName, field);
        }

        /// <summary>
        /// Registers an integer field to the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="startingValue">The value of the field when the state machine starts</param>
        public void RegisterIntField(string fieldName, int startingValue = 0)
        {
            StateMachineField field = new StateMachineField(StateMachineFieldType.Int, startingValue);
            Fields.Add(fieldName, field);
        }

        /// <summary>
        /// Registers a float field to the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="startingValue">The value of the field when the state machine starts</param>
        public void RegisterFloatField(string fieldName, float startingValue = 0)
        {
            StateMachineField field = new StateMachineField(StateMachineFieldType.Float, startingValue);
            Fields.Add(fieldName, field);
        }

        /// <summary>
        /// Registers a trigger field to the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="startingValue">The value of the field when the state machine starts</param>
        public void RegisterTriggerField(string fieldName)
        {
            StateMachineField field = new StateMachineField(StateMachineFieldType.Trigger, 0);
            Fields.Add(fieldName, field);
        }

        /// <summary>
        /// Changes the value of a previously registered boolean field of the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="value">The new value to set to the field</param>
        public void SetBoolField(string fieldName, bool value)
        {
            Fields[fieldName].Value = value ? 1 : 0;
            EvaluateTransitions();
        }

        /// <summary>
        /// Changes the value of a previously registered integer field of the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="value">The new value to set to the field</param>
        public void SetIntField(string fieldName, int value)
        {
            Fields[fieldName].Value = value;
            EvaluateTransitions();
        }

        /// <summary>
        /// Changes the value of a previously registered float field of the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="value">The new value to set to the field</param>
        public void SetFloatField(string fieldName, float value)
        {
            Fields[fieldName].Value = value;
            EvaluateTransitions();
        }

        /// <summary>
        /// Activates a previously registered trigger field of the state machine
        /// </summary>
        /// <param name="fieldName">The name of the field to register</param>
        /// <param name="value">The new value to set to the field</param>
        public void SetTrigger(string fieldName)
        {
            Fields[fieldName].Value = 1.0f;
            EvaluateTransitions();
        }
    }
}
