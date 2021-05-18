namespace Chroma.Infrastructure.FSM
{
    class EmptyState : State
    {
        public EmptyState(string name) : base(name) {}

        public override void Enter() {}

        public override void Exit() {}

        public override void Update(float deltaTime) {}

        public override void Interrupt() {}
    }
}
