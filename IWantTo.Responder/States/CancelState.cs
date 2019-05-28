
using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class CancelState : StateBase, IState
    {
        public CancelState() : base("Cancel")
        {
            AddAction(new CancelAction());
            AddAction(new ShowMenuAction());
        }
    }
}
