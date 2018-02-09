using System;
using System.Threading.Tasks;

namespace Xer.Messaginator
{
    internal class TaskUtility
    {
        internal static readonly Task CompletedTask = Task.FromResult(true);

        internal static Task FromException(Exception exception)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            tcs.TrySetException(exception);
            return tcs.Task;
        }
    }
}