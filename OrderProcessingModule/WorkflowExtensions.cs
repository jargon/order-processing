using System.Activities;
using System.Threading.Tasks;

namespace OrderProcessingModule
{
    public static class WorkflowExtensions
    {
        /// <summary>
        /// Simple extension method that starts a workflow and then waits for its completion, propagating any exception
        /// that might cause the flow to abort.
        /// </summary>
        /// <param name="wfApp">The workflow application to run.</param>
        public static void RunAndBlockUntilComplete(this WorkflowApplication wfApp)
        {
            var tcs = new TaskCompletionSource<bool>();

            wfApp.Completed += e => tcs.TrySetResult(true);
            wfApp.Aborted += e => tcs.TrySetException(e.Reason);
            wfApp.OnUnhandledException += e => { tcs.TrySetException(e.UnhandledException); return UnhandledExceptionAction.Abort; };
            wfApp.Run();

            tcs.Task.Wait();
        }
    }
}
