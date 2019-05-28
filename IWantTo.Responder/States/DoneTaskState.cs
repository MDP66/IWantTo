
using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class DoneTaskState : StateBase, IState
    {
        public DoneTaskState() : base("Done")
        {
            AddAction(new MarkTaskAsDoneAction());
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
