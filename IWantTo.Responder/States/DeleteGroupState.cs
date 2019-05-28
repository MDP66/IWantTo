
using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class DeleteGroupState : StateBase, IState
    {
        public DeleteGroupState() : base("group_del")
        {
            AddAction(new MarkGroupAsDeleteAction());
            AddAction(new ShowMyGroupsAction());
        }
        public override bool IsRequestMatch(string command)
        {
            var normilizedCommand = command.TrimStart('/').Trim().ToLower();
            var normilizedCurrentStateCommand = StateTitle.ToLower();
            return normilizedCommand.StartsWith(normilizedCurrentStateCommand);
        }
    }
}
