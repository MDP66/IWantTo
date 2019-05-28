using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class EveryMessageState : StateBase, IState
    {
        public EveryMessageState() : base("")
        {
            AddAction(new ValidateUserAction());
            AddAction(new ValidateUserGroupAction());
        }

        public override bool IsRequestMatch(string command)
        {
            return true;
        }
    }
}
