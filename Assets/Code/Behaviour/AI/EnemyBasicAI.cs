using Chroma.Behaviour.AI.Tasks;
using NPBehave;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Chroma.Assets.Code.Behaviour.AI
{
    public class EnemyBasicAI : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Root behaviorTree;

        [Inject]
        private void Inject(NavMeshAgent agent)
        {
            this.agent = agent;
        }

        void Start()
        {
            var selectDestination = new SelectRandomDestination("destination", transform);
            var moveTo = new MoveTo("destination", agent);
            var wait = new Wait(5, 2.5f);   // Wait 5s with a variance of 2.5s
            var wanderSubtree = new Sequence(selectDestination, moveTo, wait);

            behaviorTree = new Root(wanderSubtree);

            behaviorTree.Start();
        }
    }
}
