using System.Threading.Tasks;

namespace IWantTo.Responder.Model
{
    public interface IAction
    {
        /// <summary>
        /// Execute the functionality in async manner
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>Nothing</returns>
        Task ExecuteAsync(Context context);
    }
}
