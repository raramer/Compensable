using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Compensable.Tests")]

namespace Compensable
{
    public sealed partial class Compensator
    {
        private readonly CancellationToken _cancellationToken;
        private readonly SemaphoreSlim _compensationLock;
        private readonly SemaphoreSlim _executionLock;
        private readonly SemaphoreSlim _statusLock;
        private readonly ConcurrentStack<(ConcurrentStack<Func<Task>> Compensations, Tag Tag)> _taggedCompensations;
        
        public CompensatorStatus Status { get; private set; }

        public Compensator() : this(CancellationToken.None)
        {
        }

        public Compensator(CancellationToken cancellationToken)
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

        private async Task ClearCompensationsAsync()
        {
            // TODO do we need to acquire compensation lock as an additional safeguard?
            _taggedCompensations.Clear();
        }

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

        private void VerifyTagExists(Tag tag)
        {
            if (tag != null && !_taggedCompensations.Any(c => c.Tag == tag))
                throw new TagNotFoundException();
        }
    }
}