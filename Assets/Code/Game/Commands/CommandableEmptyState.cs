namespace Chroma.Game.Commands
{
    class CommandableEmptyState : CommandableState
    {
        public CommandableEmptyState(string name) : base(name) {}

        public CommandableEmptyState() : base("Entry Point") {}

        public override void Enter() {}

        public override void Exit() {}

        public override void Interrupt() {}

        public override void ProcessCommand(Command command, CommandArgs args) {}

        public override void Update(float deltaTime) {}
    }
}
