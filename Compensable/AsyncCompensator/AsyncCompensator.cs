using System;
using System.Threading;
using System.Threading.Tasks;

namespace Compensable
{
    public sealed partial class AsyncCompensator
    {
        private readonly CancellationToken _cancellationToken;
        private readonly SemaphoreSlim _compensationLock;
        private readonly SemaphoreSlim _executionLock;
        private readonly SemaphoreSlim _statusLock;
        private readonly CompensationStack<Func<Task>> _compensationStack;

        public CompensatorStatus Status { get; private set; }

        public AsyncCompensator() : this(CancellationToken.None)
        {
        }

        public AsyncCompensator(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _compensationLock = new SemaphoreSlim(1, 1);
            _executionLock = new SemaphoreSlim(1, 1);
            _statusLock = new SemaphoreSlim(1, 1);
            _compensationStack = new CompensationStack<Func<Task>>();

            Status = CompensatorStatus.Executing;
        }

        private async Task SetStatusAsync(CompensatorStatus status)
        {
            // aquire status lock, ignore cancellation token
            await _statusLock.WaitAsync().ConfigureAwait(false);

            try
            {
                // set status if greater than current status
                if (status > Status)
                    Status = status;
            }
            finally
            {
                // release lock
                _statusLock.Release();
            }
        }

        private void VerifyCanExecute()
        {
            if (Status != CompensatorStatus.Executing)
                throw new CompensatorStatusException(Status);

            _cancellationToken.ThrowIfCancellationRequested();
        }
    }
}