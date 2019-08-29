using Chroma.Behaviour.AI.Components;
using Chroma.Behaviour.AI.Decorators;
using Chroma.Behaviour.AI.Tasks;
using Chroma.Game.Containers;
using NPBehave;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Chroma.Assets.Code.Behaviour.AI
{
    public class EnemyBasicAI : MonoBehaviour
    {
        private const string randomDestinationVarname = "randomDestination";
        private const string chasingEnemyVarname = "chasingEnemy";
        private const string enemyLastSeenPosVarname = "enemyLastSeenPos";

        private NavMeshAgent agent;
        private Eyes eyes;
        private CharacterContainer character;

        private Root behaviorTree;

        [Inject]
        private void Inject(NavMeshAgent agent, Eyes eyes, CharacterContainer character)
        {
            this.agent = agent;
            this.eyes = eyes;
            this.character = character;
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
            var selectDestination = new SelectRandomDestination(randomDestinationVarname, transform);
            var moveTo = new MoveTo(agent, randomDestinationVarname);
            var wait = new Wait(5, 2.5f);   // Wait 5s with a variance of 2.5s
            var wanderST = new Sequence(selectDestination, moveTo, wait);

            var chasePlayer = new MoveTo(agent, enemyLastSeenPosVarname);
            var attack = new Wait(1.0f);    // Not really attacking
            var stopChasing = new Action(() => behaviorTree.Blackboard.Set(chasingEnemyVarname, false));
            var chaseEnemyST = new Sequence(chasePlayer, attack, stopChasing);
            var isEnemyInSight = new BlackboardCondition(chasingEnemyVarname, Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, chaseEnemyST);

            var rootSelector = new Selector(isEnemyInSight, wanderST);
            var rootChild = new IsEnemyOnSight(eyes, character, 0.1f, chasingEnemyVarname, enemyLastSeenPosVarname, rootSelector);
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
    }
}
