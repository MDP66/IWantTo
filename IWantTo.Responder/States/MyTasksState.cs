using IWantTo.Responder.Actions;
using IWantTo.Responder.Model;

namespace IWantTo.Responder.States
{
    public class MyTasksState : StateBase,IState
    {
        public MyTasksState() : base("MyTasks")
        {
            AddAction(new ShowMyTasksAction());
        }
    }
}
