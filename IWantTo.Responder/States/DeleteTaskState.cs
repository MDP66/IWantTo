
using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class DeleteTaskState : StateBase, IState
    {
        public DeleteTaskState() : base("del")
        {
            AddAction(new MarkTaskAsDeleteAction());
            AddAction(new ShowMyTasksAction());
        }

        public override bool IsRequestMatch(string command)
        {
            var normilizedCommand = command.TrimStart('/').Trim().ToLower();
            var normilizedCurrentStateCommand = StateTitle.ToLower();
            return normilizedCommand.StartsWith(normilizedCurrentStateCommand);
        }
    }
}
