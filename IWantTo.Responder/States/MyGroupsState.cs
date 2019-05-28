using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class MyGroupsState : StateBase, IState
    {
        public MyGroupsState() : base("MyGroups")
        {
            AddAction(new ShowMyGroupsAction());
        }
    }
}
