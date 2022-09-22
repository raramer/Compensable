using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
        {
            ValidateStatusIsExecuting();

            try
            {
                if (test == null)
                    throw new ArgumentNullException(nameof(test));

                if (execution == null)
                    throw new ArgumentNullException(nameof(execution));

                ValidateTag(compensateAtTag);

                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    ValidateStatusIsExecuting();

                    if (await test().ConfigureAwait(false))
                    {
                        await execution().ConfigureAwait(false);

                        if (compensation != null)
                        {
                            var compensateAtIndex = GetCompensateAtIndex(compensateAtTag);
                            _compensations.Insert(compensateAtIndex, (compensation, null));
                        }
                    }
                }
                finally
                {
                    // TODO if an exception occurs, we will compensate but there's no lock between this finally and the outer catch
                    _executionLock.Release();
                }
            }
            catch (Exception whileExecuting)
            {
                await CompensateAsync(whileExecuting).ConfigureAwait(false);
                throw;
            }
        }

        #region Two-Parameter Overloads
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

        #region Three-Parameter Overloads
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

        #region Four-Parameter Overloads
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