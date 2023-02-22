using System;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Runs the compensations in the compensation stack without requiring a failed execution.
        /// </summary>
        public void Compensate()
            => Compensate(null);

        private void Compensate(Exception whileExecuting)
        {
            // short-circuit if compensated / failed to compensate
            if (Status == CompensatorStatus.Compensated)
                return;

            if (Status == CompensatorStatus.FailedToCompensate)
                throw new CompensatorStatusException(CompensatorStatus.FailedToCompensate);

            // acquire compensation lock, ignore cancellation token on compensation
            _compensationLock.Wait();

            try
            {
                // short-circuit if compensated / failed to compensate
                if (Status == CompensatorStatus.Compensated)
                    return;

                if (Status == CompensatorStatus.FailedToCompensate)
                    throw new CompensatorStatusException(CompensatorStatus.FailedToCompensate);

                // set status
                SetStatus(CompensatorStatus.Compensating);

                // acquire execution lock, i.e. make sure nothing is still executing, ignore cancellation token on compensation
                _executionLock.Wait();

                // for each compensation
                while (_compensationStack.TryPeek(out var compensate))
                {
                    // execute compensation
                    compensate();

                    // remove compensation
                    _compensationStack.TryPop(out _);
                }

                // set status
                SetStatus(CompensatorStatus.Compensated);
            }
            catch (Exception whileCompensating)
            {
                // set status
                SetStatus(CompensatorStatus.FailedToCompensate);

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
