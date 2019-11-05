using Chroma.Behaviour.AI.Components;
using Chroma.Game.Containers;
using NPBehave;

namespace Chroma.Behaviour.AI.Decorators
{
    public class DetectEnemy : Decorator
    {
        private Eyes eyes;
        private Character character;

        private float checkInterval;
        private string enemyDetectedVarname;
        private string enemyLastSeenPosVarname;

        public DetectEnemy(
            Eyes eyes,
            Character character,
            float checkInterval,
            string enemyDetectedVarname,
            string enemyLastSeenPosVarname,
            Node decoratee)
            : base("IsEnemyOnSight", decoratee)
        {
            this.eyes = eyes;
            this.character = character;
            this.checkInterval = checkInterval;
            this.enemyDetectedVarname = enemyDetectedVarname;
            this.enemyLastSeenPosVarname = enemyLastSeenPosVarname;
            Label = "" + checkInterval + "s";
        }

        protected override void DoStart()
        {
            Clock.AddTimer(checkInterval, -1, CheckIfEnemyIsInSightAndSavePosition);
            CheckIfEnemyIsInSightAndSavePosition();
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Clock.RemoveTimer(CheckIfEnemyIsInSightAndSavePosition);
            Stopped(result);
        }

        public void CheckIfEnemyIsInSightAndSavePosition()
        {
            if(eyes.IsTargetOnSight())
            {
                Blackboard.Set(enemyDetectedVarname, true);
                Blackboard.Set(enemyLastSeenPosVarname, character.transform.position);
            }
        }
    }
}
