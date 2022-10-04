using System;
using System.Threading.Tasks;

namespace Compensable
{
    internal static class Extensions
    {
        internal static Func<Task> Awaitable(this Action action)
        {
            return action == null
                ? default(Func<Task>)
                : () => { action(); return Task.CompletedTask; };
        }

        internal static Func<T1, Task> Awaitable<T1>(this Action<T1> action)
        {
            return action == null
                ? default(Func<T1, Task>)
                : (t1) => { action(t1); return Task.CompletedTask; };
        }

        internal static Func<Task<TResult>> Awaitable<TResult>(this Func<TResult> function)
        {
            return function == null
                ? default(Func<Task<TResult>>)
                : () => Task.FromResult(function());
        }

        internal static Func<T1, Task> AwaitableIgnore<T1>(this Action action)
        {
            return action == null
                ? default(Func<T1, Task>)
                : (T1 t1) => { action(); return Task.CompletedTask; };
        }

        internal static Func<T1, Task> Ignore<T1>(this Func<Task> asyncFunction)
        {
            return asyncFunction == null
                ? default(Func<T1, Task>)
                : async (T1 t1) => await asyncFunction().ConfigureAwait(false);
        }
    }
}