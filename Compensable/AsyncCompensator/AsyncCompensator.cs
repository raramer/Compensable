using System;
using System.Collections.Concurrent;
using System.Linq;
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
        private readonly ConcurrentStack<(ConcurrentStack<Func<Task>> Compensations, Tag Tag)> _taggedCompensations;

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
            _taggedCompensations = new ConcurrentStack<(ConcurrentStack<Func<Task>> Compensations, Tag Tag)>();

            Status = CompensatorStatus.Executing;
        }

        private void AddCompensationToStack(Func<Task> compensation, Tag compensateAtTag)
        {
            if (compensateAtTag == null)
            {
                // create compensation stack
                var compensations = new ConcurrentStack<Func<Task>>();

                // add compensation
                compensations.Push(compensation);

                // add stack to tagged compensations
                _taggedCompensations.Push((compensations, null));
            }
            else
            {
                // add compensation to tagged compensation
                _taggedCompensations.First(tc => tc.Tag == compensateAtTag).Compensations.Push(compensation);
            }
        }

        private Tag AddTagToStack(Tag tag)
        {
            _taggedCompensations.Push((new ConcurrentStack<Func<Task>>(), tag));
            return tag;
        }

        private void ClearStack()
        {
            // TODO do we need to acquire compensation lock as an additional safeguard?
            _taggedCompensations.Clear();
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