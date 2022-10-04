using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task AddCompensationAsync(Func<Task> compensation, Tag compensateAtTag)
        {
            VerifyCanExecute();

            try
            {
                if (compensation == null)
                    throw new ArgumentNullException(nameof(compensation));

                VerifyTagExists(compensateAtTag);

                await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

                try
                {
                    VerifyCanExecute();

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

        #region Compensation Overloads
        public async Task AddCompensationAsync(Action compensation)
            => await AddCompensationAsync(compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task AddCompensationAsync(Func<Task> compensation)
            => await AddCompensationAsync(compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Compensation + CompensateAtTag Overload
        public async Task AddCompensationAsync(Action compensation, Tag compensateAtTag)
            => await AddCompensationAsync(compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
