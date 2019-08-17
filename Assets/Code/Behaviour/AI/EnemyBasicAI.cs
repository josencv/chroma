using Chroma.Behaviour.AI.Components;
using Chroma.Behaviour.AI.Tasks;
using NPBehave;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Chroma.Assets.Code.Behaviour.AI
{
    public class EnemyBasicAI : MonoBehaviour
    {
        private const string enemyInSightVarname = "enemyOnSight";

        private NavMeshAgent agent;
        private Eyes eyes;
        private Root behaviorTree;

        [Inject]
        private void Inject(NavMeshAgent agent, Eyes eyes)
        {
            this.agent = agent;
            this.eyes = eyes;
        }

        void Start()
        {
            BuildBehaviourTree();

#if UNITY_EDITOR
            AttachDebugger();
#endif

            behaviorTree.Start();
        }

        private void AttachDebugger()
        {
            Debugger debugger = (Debugger)gameObject.AddComponent(typeof(Debugger));
            debugger.BehaviorTree = behaviorTree;
        }

        private void BuildBehaviourTree()
        {
            var selectDestination = new SelectRandomDestination("destination", transform);
            var moveTo = new MoveTo("destination", agent);
            var wait = new Wait(5, 2.5f);   // Wait 5s with a variance of 2.5s
            var wanderST = new Sequence(selectDestination, moveTo, wait);

            var pursuitEnemyST = new Sequence(new Wait(20));
            var isEnemyInSight = new BlackboardCondition(enemyInSightVarname, Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, pursuitEnemyST);

            var rootSelector = new Selector(isEnemyInSight, wanderST);
            var rootChild = new Service(0.2f, CheckIfEnemyIsInSight, rootSelector);
            behaviorTree = new Root(rootChild);
        }

        private void OnDestroy()
        {
            StopBehaviorTree();
        }

        public void StopBehaviorTree()
        {
            if(behaviorTree != null && behaviorTree.CurrentState == Node.State.ACTIVE)
            {
                behaviorTree.Stop();
            }
        }

        // TODO: move this method somewhere else.
        // Note: I think we should use this inside a service node instead
        // of using a parallel node, because it's more straighforward
        public void CheckIfEnemyIsInSight()
        {
            if(eyes.IsTargetOnSight())
            {
                behaviorTree.Blackboard.Set(enemyInSightVarname, true);
            }
        }
    }
}
