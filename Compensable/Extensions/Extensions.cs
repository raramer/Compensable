﻿using System;
using System.Threading.Tasks;

namespace Compensable
{
    internal static class Extensions
    {

        internal static Func<Task<AsyncCompensation>> AsAsyncCompensation(this Func<Compensation> execution)
        {
            return execution == null
                ? default(Func<Task<AsyncCompensation>>)
                : () =>
                {
                    var compensation = execution();
                    Validate.ExecutionCompensation(compensation);

                    return Task.FromResult(new AsyncCompensation(compensation.Compensate));
                };
        }

        internal static Func<T1, Task<AsyncCompensation>> AsAsyncCompensation<T1>(this Func<T1, Compensation> execution)
        {
            return execution == null
                ? default(Func<T1, Task<AsyncCompensation>>)
                : (T1 item) =>
                {
                    var compensation = execution(item);
                    Validate.ExecutionCompensation(compensation);

                    return Task.FromResult(new AsyncCompensation(compensation.Compensate));
                };
        }

        internal static Func<Task<AsyncCompensation<TResult>>> AsAsyncCompensation<TResult>(this Func<Compensation<TResult>> execution)
        {
            return execution == null
                ? default(Func<Task<AsyncCompensation<TResult>>>)
                : () =>
                {
                    var compensation_result = execution();
                    Validate.ExecutionCompensation(compensation_result);

                    return Task.FromResult(new AsyncCompensation<TResult>(compensation_result.Result, compensation_result.Compensate));
                };
        }

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

        internal static Func<T1, Task<TResult>> Awaitable<T1, TResult>(this Func<T1, TResult> function)
        {
            return function == null
                ? default(Func<T1, Task<TResult>>)
                : (t1) => Task.FromResult(function(t1));
        }

        internal static Func<T1, Task> AwaitableIgnoreParameter<T1>(this Action action)
        {
            return action == null
                ? default(Func<T1, Task>)
                : (T1 t1) => { action(); return Task.CompletedTask; };
        }

        internal static Action<T1> IgnoreParameter<T1>(this Action action)
        {
            return action == null
                ? default(Action<T1>)
                : (T1 t1) => action();
        }

        internal static Func<T1, Task> IgnoreParameter<T1>(this Func<Task> asyncFunction)
        {
            return asyncFunction == null
                ? default(Func<T1, Task>)
                : async (T1 t1) => await asyncFunction().ConfigureAwait(false);
        }
    }
}