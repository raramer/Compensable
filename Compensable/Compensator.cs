using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Compensable.Tests")]

namespace Compensable
{
    public sealed partial class Compensator
    {
        internal const string IsExecuting = "is executing";
        internal const string IsCompensating = "is compensating";
        internal const string WasCompensated = "was compensated";
        internal const string FailedToCompensate = "failed to compensate";

        private readonly SemaphoreSlim _compensationLock = new SemaphoreSlim(1, 1);
        private readonly List<Func<Task>> _compensations = new List<Func<Task>>();
        private readonly SemaphoreSlim _executionLock = new SemaphoreSlim(1, 1);
        
        internal string Status { get; private set; } = IsExecuting;

        public async Task CompensateAsync() => await CompensateAsync(null).ConfigureAwait(false);

        private async Task CompensateAsync(Exception whileExcuting)
        {
            // short-circuit if compensated
            if (Status == WasCompensated || Status == FailedToCompensate)
                return;

            // acquire compensation lock
            await _compensationLock.WaitAsync().ConfigureAwait(false);

            try
            {
                // short-circuit if compensated
                if (Status == WasCompensated || Status == FailedToCompensate)
                    return;

                Status = IsCompensating;

                // acquire execution lock, i.e. make sure nothing is still executing
                await _executionLock.WaitAsync().ConfigureAwait(false);

                // compensate (work backwards through list)
                while (_compensations.Count > 0)
                {
                    // get last index
                    var lastIndex = _compensations.Count - 1;

                    // execute compensation
                    await _compensations[lastIndex]().ConfigureAwait(false);

                    // remove execution from list
                    _compensations.RemoveAt(lastIndex);
                }

                Status = WasCompensated;
            }
            catch (Exception whileCompensating)
            {
                Status = FailedToCompensate;

                throw (whileExcuting == null)
                    ? new CompensationException(whileCompensating)
                    : new CompensationException(whileCompensating, whileExcuting);
            }
            finally
            {
                // release locks
                _executionLock.Release();
                _compensationLock.Release();
            }
        }

        private void VerifyStatusIsExecuting([CallerMemberName] string caller = null)
        {
            if (Status != IsExecuting)
                throw new InvalidOperationException($"Cannot call {caller} when compensator {Status}.");
        }
    }
}