﻿using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task DoAsync(Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
        {
            VerifyCanExecute();

            try
            {
                if (execution == null)
                    throw new ArgumentNullException(nameof(execution));

                VerifyTagExists(compensateAtTag);

                await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

                try
                {
                    VerifyCanExecute();

                    await execution().ConfigureAwait(false);

                    if (compensation != null)
                        AddCompensationToStack(compensation, compensateAtTag);
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

        #region Execution Overloads
        public async Task DoAsync(Action execution)
            => await DoAsync(execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        public async Task DoAsync(Func<Task> execution)
            => await DoAsync(execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation Overloads
        public async Task DoAsync(Action execution, Action compensation)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoAsync(Action execution, Func<Task> compensation)
            => await DoAsync(execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        public async Task DoAsync(Func<Task> execution, Action compensation)
            => await DoAsync(execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task DoAsync(Func<Task> execution, Func<Task> compensation)
            => await DoAsync(execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        public async Task DoAsync(Action execution, Action compensation, Tag compensateAtTag)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task DoAsync(Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoAsync(execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);


        public async Task DoAsync(Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoAsync(execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}