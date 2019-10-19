using Chroma.Behaviour.Attack;
using NPBehave;

namespace Chroma.Behaviour.AI.Tasks
{
    public class Attack : Task
    {
        private SimpleAttack attack;

        public Attack(SimpleAttack attack) : base("Attack")
        {
            this.attack = attack;
        }

        protected override void DoStart()
        {
            attack.StartAttack();
            Stopped(true);
        }

        protected override void DoStop()
        {
            Stopped(false);
        }
    }
}
