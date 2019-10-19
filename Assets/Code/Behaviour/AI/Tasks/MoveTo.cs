using NPBehave;
using UnityEngine;
using UnityEngine.AI;

namespace Chroma.Behaviour.AI.Tasks
{
    /// <summary>
    /// Moves a NavAgent to the destination specified in the blackboard variable "destinationVarname".
    /// The tasks succeeds if the agent reaches the destination (with a margin of error) and fails if the
    /// distance is not shortened in "MaxDistanceChecks" checks.
    /// </summary>
    public class MoveTo : Task
    {
        /// <summary>
        /// Used to check wheter the nav agent has reached it's destination
        /// Note: we use squared distance because it is faster to get the squared distance between 2 vectors
        /// </summary>
        private const float SqrMinDistanceThreshold = 0.01f;

        /// <summary>
        /// The frequency (interval in seconds) the script checks if the agent reached its destination
        /// </summary>
        private const float StepCheckInterval = 0.1f;

        /// <summary>
        /// A base number used to define the max distance checks, depending of the check interval.
        /// The idea behind of this variable is to keep a number of maximum failed checks that is
        /// inversely proportional to the step check interval
        /// </summary>
        private const int MaxDistanceChecksBase = 30;

        /// <summary>
        /// The max failed checks to accept before the task is stopped with a failed result
        /// </summary>
        private const int MaxDistanceChecks = MaxDistanceChecksBase / (int) (StepCheckInterval * 1000);

        private NavMeshAgent agent;

        private string destinationVarname;
        private float lastSqrDistance;
        private int failedChecks;
        private float sqrRange;

        public MoveTo(NavMeshAgent agent, string destinationVarname, float range = 0) : base("MoveTo")
        {
            this.destinationVarname = destinationVarname;
            this.agent = agent;
            sqrRange = range * range;
            lastSqrDistance = float.MaxValue;
            failedChecks = 0;
        }

        private void Reset()
        {
            lastSqrDistance = float.MaxValue;
            failedChecks = 0;
        }

        protected override void DoStart()
        {
            Reset();
            Clock.AddTimer(StepCheckInterval, -1, CheckIfDestinationWasReached);
            CheckIfDestinationWasReached();
        }

        protected override void DoStop()
        {
            CleanAndStop(false);
        }

        private void CheckIfDestinationWasReached()
        {
            // Update agent destination before doing calculations
            Vector3 newDestination = Blackboard.Get<Vector3>(destinationVarname);
            if(agent.destination != newDestination)
            {
                Reset();
                agent.destination = Blackboard.Get<Vector3>(destinationVarname);
            }

            float sqrDistance = Vector3.SqrMagnitude(agent.destination - agent.transform.position);
            bool hasDistanceImproved = lastSqrDistance - sqrDistance > 0;

            if(sqrDistance < SqrMinDistanceThreshold + sqrRange)
            {
                CleanAndStop(true);
                return;
            }

            if(hasDistanceImproved)
            {
                lastSqrDistance = sqrDistance;
            }
            else
            {
                failedChecks++;
                if(failedChecks >= MaxDistanceChecks)
                {
                    CleanAndStop(false);
                }
            }
        }

        private void CleanAndStop(bool result)
        {
            // We add this check because when the game is closed this method is
            // called after the agent is disabled, which causes an error.
            if(agent.isActiveAndEnabled)
            {
                agent.destination = agent.transform.position;
            }

            Clock.RemoveTimer(CheckIfDestinationWasReached);
            Stopped(result);
        }
    }
}
