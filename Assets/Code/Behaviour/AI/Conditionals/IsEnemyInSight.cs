using Chroma.Behaviour.AI.Components;
using NPBehave;

namespace Chroma.Behaviour.AI.Coinditionals
{
    public class IsEnemyInSight : Task
    {
        private Eyes eyes;

        public IsEnemyInSight(Eyes eyes)
            : base("IsEnemyInSight")
        {
            this.eyes = eyes;
        }

        protected override void DoStart()
        {
            if(eyes.IsTargetOnSight())
            {
                Stopped(true);
            }
            else
            {
                Stopped(false);
            }
        }

        override protected void DoStop()
        {
            Stopped(false);
        }
    }
}
