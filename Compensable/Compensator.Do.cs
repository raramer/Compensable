using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task DoAsync(Func<Task> execution, Func<Task> compensation)
        {
            VerifyStatusIsExecuting();

            try
            {
                if (execution == null)
                    throw new ArgumentException($"{nameof(execution)} is required.");

                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    VerifyStatusIsExecuting();

                    await execution().ConfigureAwait(false);

                    if (compensation != null)
                        _compensations.Add(compensation);
                }
                finally
                {
                    _executionLock.Release();
                }
            }
            catch (Exception whileExecuting)
            {
                await CompensateAsync(whileExecuting).ConfigureAwait(false);
                throw;
            }
        }


        public async Task DoAsync(Action execution)
            => await DoAsync(execution.Awaitable(), default(Func<Task>)).ConfigureAwait(false);

        public async Task DoAsync(Action execution, Action compensation)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable()).ConfigureAwait(false);

        public async Task DoAsync(Action execution, Func<Task> compensation)
            => await DoAsync(execution.Awaitable(), compensation).ConfigureAwait(false);

        public async Task DoAsync(Func<Task> execution)
            => await DoAsync(execution, default(Func<Task>)).ConfigureAwait(false);

        public async Task DoAsync(Func<Task> execution, Action compensation)
            => await DoAsync(execution, compensation.Awaitable()).ConfigureAwait(false);
    }
}