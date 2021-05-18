using System.Collections.Generic;

namespace Chroma.Infrastructure.FSM
{
    /// <summary>
    /// A state of a state machine. It has transitions that points to other states than can be transited to
    /// if the transition conditions are met
    /// </summary>
    abstract class State
    {
        public string Name{ get; }
        public List<StateTransition> Transitions { get; }

        public State(string name)
        {
            Name = name;
            Transitions = new List<StateTransition>();
        }

        /// <summary>
        /// Adds a transition to the state
        /// </summary>
        /// <param name="transition"></param>
        public virtual void AddTransition(StateTransition transition)
        {
            Transitions.Add(transition);
        }

        /// <summary>
        /// Called when the state starts or the machine "enters" to this state
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// Update call to be used inside the game update loop
        /// <param name="deltaTime">Time since last frame</param>
        /// </summary>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Called when the state ends or the machine "exits" this state
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// Interrupts the current state. Normally used when an external event interrupts the natural course
        /// of this state (ej: the character got hit)
        /// </summary>
        public abstract void Interrupt();
    }
}
