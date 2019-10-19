using NPBehave;

namespace Chroma.Behaviour.AI.Decorators
{
    public class OnFailure : Decorator
    {
        private System.Action action;

        public OnFailure(
            System.Action action,
            Node decoratee)
            : base("OnFailure", decoratee)
        {
            this.action = action;
        }

        protected override void DoStart()
        {
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            action();
            Stopped(result);
        }
    }
}
