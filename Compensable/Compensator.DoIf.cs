using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Func<Task> compensation)
        {
            VerifyStatusIsExecuting();

            try
            {
                if (test == null)
                    throw new ArgumentException($"{nameof(execution)} is required.");

                if (execution == null)
                    throw new ArgumentException($"{nameof(execution)} is required.");

                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    VerifyStatusIsExecuting();

                    if (await test().ConfigureAwait(false))
                    {
                        await execution().ConfigureAwait(false);

                        if (compensation != null)
                            _compensations.Add(compensation);
                    }
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


        public async Task DoIfAsync(bool test, Action execution)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), default(Func<Task>)).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Action execution, Action compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation.Awaitable()).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Action execution, Func<Task> compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution)
            => await DoIfAsync(() => Task.FromResult(test), execution, default(Func<Task>)).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution, Action compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation.Awaitable()).ConfigureAwait(false);


        public async Task DoIfAsync(Func<bool> test, Action execution)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), default(Func<Task>)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Action execution, Action compensation)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation.Awaitable()).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Action execution, Func<Task> compensation)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution)
            => await DoIfAsync(test.Awaitable(), execution, default(Func<Task>)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Action compensation)
            => await DoIfAsync(test.Awaitable(), execution, compensation.Awaitable()).ConfigureAwait(false);



        public async Task DoIfAsync(Func<Task<bool>> test, Action execution)
            => await DoIfAsync(test, execution.Awaitable(), default(Func<Task>)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Action compensation)
            => await DoIfAsync(test, execution.Awaitable(), compensation.Awaitable()).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Func<Task> compensation)
            => await DoIfAsync(test, execution.Awaitable(), compensation).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution)
            => await DoIfAsync(test, execution, default(Func<Task>)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Action compensation)
            => await DoIfAsync(test, execution, compensation.Awaitable()).ConfigureAwait(false);
    }
}