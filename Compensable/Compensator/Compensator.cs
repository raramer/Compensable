using System;
using System.Threading;

namespace Compensable
{
    public sealed partial class Compensator
    {
        private readonly CancellationToken _cancellationToken;
        private readonly SemaphoreSlim _compensationLock;
        private readonly SemaphoreSlim _executionLock;
        private readonly SemaphoreSlim _statusLock;
        private readonly CompensationStack<Action> _compensationStack;

        /// <summary>
        /// The status of the compensator.
        /// </summary>
        public CompensatorStatus Status { get; private set; }

        /// <summary>
        /// Creates a new synchronous compensator.
        /// </summary>
        public Compensator() : this(CancellationToken.None)
        {
        }

        /// <summary>
        /// Creates a new synchronous compensator.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        public Compensator(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _compensationLock = new SemaphoreSlim(1, 1);
            _executionLock = new SemaphoreSlim(1, 1);
            _statusLock = new SemaphoreSlim(1, 1);
            _compensationStack = new CompensationStack<Action>();

            Status = CompensatorStatus.Executing;
        }

        private void SetStatus(CompensatorStatus status)
        {
            // aquire status lock, ignore cancellation token
            _statusLock.Wait();

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