
using IWantTo.Responder.Model;
using IWantTo.Responder.States;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace IWantTo.Responder
{
    public class StateProcessor
    {
        public async Task ProcessAsync(Context context)
        {
            var states = new List<IState>
            {
                new StartState(),
                new CancelState(),
                new MyTasksState(),
                new MyGroupsState(),
                new DoneTaskState(),
                new DeleteTaskState(),
                new DeleteGroupState(),
            };

            var message = context.Update.Message.Text;
            var messageHandled = false;
            await new EveryMessageState().HandleAsync(context);
            if (context.Update.Message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private) return;

            foreach (var state in states)
            {
                if (state.IsRequestMatch(message))
                {
                    await state.HandleAsync(context);
                    messageHandled = true;
                    break;
                }
            }

            if (!messageHandled)
            {
                await new NewState().HandleAsync(context);
            }
        }
    }
}
