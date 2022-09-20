using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        private async Task AddCompensationAsync(Func<Task> compensation)
        {
            VerifyStatusIsExecuting();

            try
            {
                if (compensation == null)
                    throw new ArgumentException($"{nameof(compensation)} is required.");

                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    VerifyStatusIsExecuting();

                    _compensations.Add(compensation);
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


        private async Task AddCompensationAsync(Action compensation)
            => await AddCompensationAsync(compensation.Awaitable()).ConfigureAwait(false);

    }
}
