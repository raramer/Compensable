using System;
using System.Threading.Tasks;

namespace Compensable
{
    public sealed class AsyncCompensation<TResult> : IExecutionCompensation
    {
        private readonly Func<Task> _compensation;

        internal bool HasCompensation => _compensation != null;

        public TResult Result { get; }

        public AsyncCompensation(TResult result, Action compensation)
        {
            Result = result;
            _compensation = compensation.Awaitable();
        }

        public AsyncCompensation(TResult result, Action<TResult> compensation)
        {
            Result = result;
            _compensation = compensation == null
                ? default(Func<Task>)
                : () =>
                {
                    compensation(result);
                    return Task.CompletedTask;
                };
        }

        public AsyncCompensation(TResult result, Func<Task> compensation)
        {
            Result = result;
            _compensation = compensation;
        }

        public AsyncCompensation(TResult result, Func<TResult, Task> compensation)
        {
            Result = result;
            _compensation = compensation == null 
                ? default(Func<Task>) 
                : async () => await compensation(result).ConfigureAwait(false);
        }

        public async Task CompensateAsync()
        { 
            if (HasCompensation)
                await _compensation().ConfigureAwait(false);
        }

        public static AsyncCompensation<TResult> Noop(TResult result) => new AsyncCompensation<TResult>(result, default(Func<Task>));

        public static implicit operator TResult(AsyncCompensation<TResult> compensableResponse) => compensableResponse.Result;
    }
}
