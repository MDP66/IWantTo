
using IWantTo.Responder.Actions;
using IWantTo.Responder.Entities;
using IWantTo.Responder.Model;

using System;
using System.Threading.Tasks;

namespace IWantTo.Responder.States
{
    public class NewState : StateBase, IState
    {
        public NewState() : base("new")
        { }

        public override async Task HandleAsync(Context context)
        {
            var userState = GetUserState(context.User);
            switch (userState)
            {
                case TaskStateTypes.Empty:
                    await HandleStartNewActionAsync(context);                    
                    break;
                case TaskStateTypes.New:
                    await HandleNewActionAsync(context);
                    break;
                case TaskStateTypes.Date:
                    await HandleDateActionAsync(context);
                    break;
                case TaskStateTypes.Privacy:
                    await HandlePrivacyActionAsync(context);
                    break;
                case TaskStateTypes.UnKnown:
                    break;
                default:
                    break;
            }
        }

        private async Task HandleStartNewActionAsync(Context context)
        {
            await new StartNewTaskAction().ExecuteAsync(context);
            context.SetArgs(TaskStateTypes.New);
            await new MarkTaskAction().ExecuteAsync(context);
        }

        private async Task HandleNewActionAsync(Context context)
        {
            await new SetNewTaskMessageAction().ExecuteAsync(context);

            context.SetArgs(TaskStateTypes.Date);
            await new MarkTaskAction().ExecuteAsync(context);

            await new SetTaskDateMessageAction().ExecuteAsync(context);
        }
        private async Task HandleDateActionAsync(Context context)
        {
            await new SetNewTaskDateAction().ExecuteAsync(context);

            context.SetArgs(TaskStateTypes.Privacy);
            await new MarkTaskAction().ExecuteAsync(context);

            await new SetTaskPrivacyMessageAction().ExecuteAsync(context);
        }

        private async Task HandlePrivacyActionAsync(Context context)
        {
            await new SetNewTaskPrivacyAction().ExecuteAsync(context);
            context.SetArgs(TaskStateTypes.Empty);
            await new MarkTaskAction().ExecuteAsync(context);

            await new ShowDoneSaveTaskMessageAction().ExecuteAsync(context);
        }


        private TaskStateTypes GetUserState(User user)
        {
            try
            {
                Enum.TryParse(typeof(TaskStateTypes), user.State, true, out var state);
                return (TaskStateTypes)state;
            }
            catch (Exception)
            {
                return TaskStateTypes.UnKnown;
            }
        }

        public override bool IsRequestMatch(string command)
        {
            return true;
        }
    }
}
