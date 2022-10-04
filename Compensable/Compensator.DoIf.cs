using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
        {
            VerifyCanExecute();

            try
            {
                if (test == null)
                    throw new ArgumentNullException(nameof(test));

                if (execution == null)
                    throw new ArgumentNullException(nameof(execution));

                VerifyTagExists(compensateAtTag);

                await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

                try
                {
                    VerifyCanExecute();

                    if (await test().ConfigureAwait(false))
                    {
                        await execution().ConfigureAwait(false);

                        if (compensation != null)
                            AddCompensationToStack(compensation, compensateAtTag);
                    }
                }
                catch
                {
                    await SetStatusAsync(CompensatorStatus.FailedToExecute).ConfigureAwait(false);
                    throw;
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

        #region Test + Execution Overloads
        public async Task DoIfAsync(bool test, Action execution)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution)
            => await DoIfAsync(() => Task.FromResult(test), execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);


        public async Task DoIfAsync(Func<bool> test, Action execution)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution)
            => await DoIfAsync(test.Awaitable(), execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);


        public async Task DoIfAsync(Func<Task<bool>> test, Action execution)
            => await DoIfAsync(test, execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution)
            => await DoIfAsync(test, execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Test + Execution + Compensation Overloads
        public async Task DoIfAsync(bool test, Action execution, Action compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Action execution, Func<Task> compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution, Action compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution, Func<Task> compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation, default(Tag)).ConfigureAwait(false);


        public async Task DoIfAsync(Func<bool> test, Action execution, Action compensation)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Action execution, Func<Task> compensation)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Action compensation)
            => await DoIfAsync(test.Awaitable(), execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Func<Task> compensation)
            => await DoIfAsync(test.Awaitable(), execution, compensation, default(Tag)).ConfigureAwait(false);


        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Action compensation)
            => await DoIfAsync(test, execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Func<Task> compensation)
            => await DoIfAsync(test, execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Action compensation)
            => await DoIfAsync(test, execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Func<Task> compensation)
            => await DoIfAsync(test, execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Test + Execution + Compensation + CompensateAtTag Overloads
        public async Task DoIfAsync(bool test, Action execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(bool test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation, compensateAtTag).ConfigureAwait(false);


        public async Task DoIfAsync(Func<bool> test, Action execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution, compensation, compensateAtTag).ConfigureAwait(false);


        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test, execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(test, execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);

        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test, execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}