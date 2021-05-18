using System.Collections.Generic;

namespace Chroma.Infrastructure.FSM
{
    /// <summary>
    /// Represents a directional connection between two states, where one can transit to the other
    /// if the transition conditions are met
    /// </summary>
    class StateTransition
    {
        public State From { get; set; }
        public State To { get; set; }
        private List<TransitionCondition> conditions;   // The list of conditions that have to be met to be able to change to the pointed state

        /// <summary>
        /// Initializes an instance of the StateTransition class
        /// </summary>
        /// <param name="from">The state that will point to the other state</param>
        /// <param name="to">The pointed state</param>
        public StateTransition(State from, State to)
        {
            conditions = new List<TransitionCondition>();
            From = from;
            To = to;
        }

        /// <summary>
        /// Adds a condition to the transition conditions list.
        /// </summary>
        /// <param name="condition"></param>
        public void AddCondition(TransitionCondition condition)
        {
            conditions.Add(condition);
        }

        /// <summary>
        /// Check if the transition can be applied (i.e. all the transitions conditions are met).
        /// </summary>
        /// <param name="fields">the list of fields used to check if the conditions are met.</param>
        /// <returns>True if all conditions are met. False otherwise.</returns>
        public bool ShouldApplyTransition(Dictionary<string, StateMachineField> fields)
        {
            foreach (TransitionCondition condition in conditions)
            {
                if (!condition.IsConditionMet(fields[condition.FieldName].Value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Applies the transition
        /// </summary>
        public void Apply()
        {
            To.Enter();
        }
    }
}
