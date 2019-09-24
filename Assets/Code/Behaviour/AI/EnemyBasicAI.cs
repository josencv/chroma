using Chroma.Behaviour.AI.Components;
using Chroma.Behaviour.AI.Decorators;
using Chroma.Behaviour.AI.Tasks;
using Chroma.Behaviour.Attack;
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
        private SimpleAttack simpleAttack;

        private Root behaviorTree;

        [Inject]
        private void Inject(NavMeshAgent agent, Eyes eyes, CharacterContainer character, SimpleAttack simpleAttack)
        {
            this.agent = agent;
            this.eyes = eyes;
            this.character = character;
            this.simpleAttack = simpleAttack;
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
            // Wander subtree
            var selectDestination = new SelectRandomDestination(randomDestinationVarname, transform);
            var moveTo = new MoveTo(agent, randomDestinationVarname);
            var wait = new Wait(5, 2.5f);   // Wait 5s with a variance of 2.5s
            var wanderST = new Sequence(selectDestination, moveTo, wait);
            
            // Chase and attack enemy subtree
            
            // Note: this assumes that the range of the attack will not change, ever. In the future we should
            // write an "Approach" task that checks the attack distance every time
            var chasePlayer = new MoveTo(agent, enemyLastSeenPosVarname, simpleAttack.AttackApproachDistance);
            var attack = new Attack(simpleAttack);
            var waitAfterAttack = new Wait((simpleAttack.AttackRate + 10) / 1000); // We add 10 milliseconds as a safety margin
            var chaseEnemySequence = new Sequence(chasePlayer, attack, waitAfterAttack);
            var chaseEnemyST = new Repeater(chaseEnemySequence);

            // Root tree (checks if enemy is on sight in order to chenge from the wander subtree to the chase subtree
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
