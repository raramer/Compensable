using System;
using System.Threading.Tasks;

namespace Compensable
{
    public sealed class AsyncCompensation : IExecutionCompensation
    {
        private readonly Func<Task> _compensation;

        internal bool HasCompensation => _compensation != null;

        public AsyncCompensation(Action compensation)
        {
            _compensation = compensation.Awaitable();
        }

        public AsyncCompensation(Func<Task> compensation)
        {
            _compensation = compensation;
        }

        public async Task CompensateAsync()
        { 
            if (HasCompensation)
                await _compensation().ConfigureAwait(false);
        }

        public static AsyncCompensation Noop => new AsyncCompensation(default(Func<Task>));
    }
}
