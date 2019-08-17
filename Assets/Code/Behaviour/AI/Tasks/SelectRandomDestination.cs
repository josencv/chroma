using NPBehave;
using UnityEngine;
using UnityEngine.AI;

namespace Chroma.Behaviour.AI.Tasks
{
    public class SelectRandomDestination : Task
    {
        private const float MaxRetries = 10.0f;
        private const float NavMeshSampelRadius = 4.0f;

        // TODO: get this values from somewhere else?
        private const float MinWalkDistance = 8.0f;
        private const float MaxWalkDistance = 8.0f;

        private string destinationVarname;
        private Transform selfTransform;
        private int failedTries;

        public SelectRandomDestination(string destinationVarname, Transform selfTransform)
            : base("SelectRandomDestination")
        {
            this.destinationVarname = destinationVarname;
            this.selfTransform = selfTransform;
        }

        private void Initialize()
        {
            failedTries = 0;
        }

        protected override void DoStart()
        {
            Initialize();

            // Note: because getting the nearest point to the nav mesh can fail, we try
            // to select random point until the max retries amount has been reached.
            while(!TrySelectRandomPoint() && failedTries < MaxRetries)
            {
                failedTries++;
            }

            if(failedTries < MaxRetries)
            {
                Stopped(true);
            }
            else
            {
                Stopped(false);
            }
        }

        /// <summary>
        /// Tries to select a random point in the navmesh. If a point is found,
        /// sets the destination point in the blackboard.
        /// </summary>
        /// <returns>True if a point is found. False otherwise.</returns>
        private bool TrySelectRandomPoint()
        {
            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 movement = new Vector3(direction.x, 0, direction.y) * UnityEngine.Random.Range(MinWalkDistance, MaxWalkDistance);
            Vector3 destination = selfTransform.position + movement;

            NavMeshHit hit;
            bool pointFound = NavMesh.SamplePosition(destination, out hit, NavMeshSampelRadius, NavMesh.AllAreas);

            if(pointFound)
            {
                Blackboard.Set(destinationVarname, hit.position);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
