
using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class StartState : StateBase, IState
    {
        public StartState() : base("Start")
        {
            AddAction(new ShowMenuAction());
        }

    }
}
