
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IWantTo.Responder.Model
{
    /// <summary>
    /// Represent each action
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// State title
        /// </summary>
        string StateTitle { get; }

        /// <summary>
        /// State available actions
        /// </summary>
        List<IAction> Actions { get; }

        /// <summary>
        /// Action handler
        /// </summary>
        /// <param name="context">execution context</param>
        /// <returns>Nothing</returns>
        Task HandleAsync(Context context);

        /// <summary>
        /// Validate is this request blong to this state
        /// </summary>
        /// <returns></returns>
        bool IsRequestMatch(string command);
    }
}
