using IWantTo.Responder.Model;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace IWantTo.Responder.States
{
    public class StateBase
    {

        private List<IAction> _Actions;
        public string StateTitle { get; private set; }
        public List<IAction> Actions => _Actions;

        public StateBase(string stateTitle)
        {
            StateTitle = stateTitle;
            _Actions = new List<IAction>();
        }
        public virtual async Task HandleAsync(Context context)
        {
            foreach (var action in _Actions)
            {
                await action.ExecuteAsync(context);
            }
        }

        public virtual bool IsRequestMatch(string command)
        {
            return StateTitle.ToLower() == command.TrimStart('/').Trim().ToLower();
        }

        public void AddAction(IAction action)
        {
            if (action != null)
                _Actions.Add(action);
        }
    }
}
