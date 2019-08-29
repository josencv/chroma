using Chroma.Behaviour.AI.Components;
using Chroma.Game.Containers;
using NPBehave;
using UnityEngine;

namespace Chroma.Behaviour.AI.Decorators
{
    public class IsEnemyOnSight : Decorator
    {
        private Eyes eyes;
        private CharacterContainer character;

        private float interval;
        private string enemyDetectedVarname;
        private string enemyLastSeenPosVarname;

        public IsEnemyOnSight(
            Eyes eyes,
            CharacterContainer character,
            float interval,
            string enemyDetectedVarname,
            string enemyLastSeenPosVarname,
            Node decoratee)
            : base("IsEnemyOnSight", decoratee)
        {
            this.eyes = eyes;
            this.character = character;
            this.interval = interval;
            this.enemyDetectedVarname = enemyDetectedVarname;
            this.enemyLastSeenPosVarname = enemyLastSeenPosVarname;
            Label = "" + interval + "s";
        }

        protected override void DoStart()
        {
            Clock.AddTimer(interval, -1, CheckIfEnemyIsInSight);
            CheckIfEnemyIsInSight();
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Clock.RemoveTimer(CheckIfEnemyIsInSight);
            Stopped(result);
        }

        public void CheckIfEnemyIsInSight()
        {
            if(eyes.IsTargetOnSight())
            {
                Blackboard.Set(enemyDetectedVarname, true);
                Blackboard.Set(enemyLastSeenPosVarname, character.transform.position);
            }
        }
    }
}
