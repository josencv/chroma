using Chroma.Infrastructure.FSM;

namespace Chroma.Game.Commands
{
    abstract class CommandableState : State
    {
        protected CommandableState(string name) : base(name) {}

        public abstract void ProcessCommand(Command command, CommandArgs args);
    }
}
