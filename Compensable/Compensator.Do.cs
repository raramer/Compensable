using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task DoAsync(Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
        {
            ValidateStatusIsExecuting();

            try
            {
                if (execution == null)
                    throw new ArgumentNullException(nameof(execution));

                ValidateTag(compensateAtTag);

                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    ValidateStatusIsExecuting();

                    await execution().ConfigureAwait(false);

                    if (compensation != null)
                    {
                        var compensateAtIndex = GetCompensateAtIndex(compensateAtTag);
                        _compensations.Insert(compensateAtIndex, (compensation, null));
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

        #region One-Parameter Overloads
        public async Task DoAsync(Action execution)
            => await DoAsync(execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        public async Task DoAsync(Func<Task> execution)
            => await DoAsync(execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Two-Parameter Overloads
        public async Task DoAsync(Action execution, Action compensation)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoAsync(Action execution, Func<Task> compensation)
            => await DoAsync(execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        public async Task DoAsync(Func<Task> execution, Action compensation)
            => await DoAsync(execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoAsync(Func<Task> execution, Func<Task> compensation)
            => await DoAsync(execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Three-Parameter Overloads
        public async Task DoAsync(Action execution, Action compensation, Tag compensateAtTag)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoAsync(Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoAsync(execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);


        public async Task DoAsync(Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoAsync(execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}