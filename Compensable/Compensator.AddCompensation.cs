using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task AddCompensationAsync(Func<Task> compensation, Tag compensateAtTag)
        {
            ValidateStatusIsExecuting();

            try
            {
                if (compensation == null)
                    throw new ArgumentNullException(nameof(compensation));

                ValidateTag(compensateAtTag);

                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    ValidateStatusIsExecuting();

                    var compensateAtIndex = GetCompensateAtIndex(compensateAtTag);
                    _compensations.Insert(compensateAtIndex, (compensation, null));
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
        public async Task AddCompensationAsync(Action compensation)
            => await AddCompensationAsync(compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task AddCompensationAsync(Func<Task> compensation)
            => await AddCompensationAsync(compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Two-Parameter Overloads
        public async Task AddCompensationAsync(Action compensation, Tag compensateAtTag)
            => await AddCompensationAsync(compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
