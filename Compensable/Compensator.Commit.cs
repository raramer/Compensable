using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task CommitAsync()
        {
            VerifyCanExecute();

            try
            {
                await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

                try
                {
                    VerifyCanExecute();

                    ClearCompensations();
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
    }
}
