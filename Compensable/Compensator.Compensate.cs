using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task CompensateAsync()
            => await CompensateAsync(null).ConfigureAwait(false);

        private async Task CompensateAsync(Exception whileExecuting)
        {
            // short-circuit if compensated / failed to compensate
            if (Status == CompensatorStatus.Compensated)
                return;

            if (Status == CompensatorStatus.FailedToCompensate)
                throw new CompensatorStatusException(CompensatorStatus.FailedToCompensate);

            // acquire compensation lock, ignore cancellation token on compensation
            await _compensationLock.WaitAsync().ConfigureAwait(false);

            try
            {
                // short-circuit if compensated / failed to compensate
                if (Status == CompensatorStatus.Compensated)
                    return;

                if (Status == CompensatorStatus.FailedToCompensate)
                    throw new CompensatorStatusException(CompensatorStatus.FailedToCompensate);

                // set status
                await SetStatusAsync(CompensatorStatus.Compensating).ConfigureAwait(false);

                // acquire execution lock, i.e. make sure nothing is still executing, ignore cancellation token on compensation
                await _executionLock.WaitAsync().ConfigureAwait(false);

                // compensate compensation tags
                while (_taggedCompensations.TryPeek(out var taggedCompensations))
                {
                    while (taggedCompensations.Compensations.TryPeek(out var compensateAsync))
                    {
                        // execute compensation
                        await compensateAsync().ConfigureAwait(false);

                        // remove compensation
                        taggedCompensations.Compensations.TryPop(out _);
                    }

                    // removed tagged compensation
                    _taggedCompensations.TryPop(out _);
                }

                // set status
                await SetStatusAsync(CompensatorStatus.Compensated).ConfigureAwait(false);
            }
            catch (Exception whileCompensating)
            {
                // set status
                await SetStatusAsync(CompensatorStatus.FailedToCompensate).ConfigureAwait(false);

                // throw compensation exception
                throw new CompensationException(
                    whileCompensating: whileCompensating,
                    whileExecuting: whileExecuting);
            }
            finally
            {
                // release locks
                _executionLock.Release();
                _compensationLock.Release();
            }
        }
    }
}
