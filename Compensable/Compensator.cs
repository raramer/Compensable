using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Compensable.Tests")]

namespace Compensable
{
    public sealed partial class Compensator
    {
        private readonly SemaphoreSlim _compensationLock = new SemaphoreSlim(1, 1);
        private readonly List<(Func<Task> Compensate, Tag Tag)> _compensations = new List<(Func<Task> Compensate, Tag tag)>();
        private readonly SemaphoreSlim _executionLock = new SemaphoreSlim(1, 1);

        public CompensatorStatus Status { get; private set; }

        public Compensator()
        {
            _compensationLock = new SemaphoreSlim(1, 1);
            _compensations = new List<(Func<Task> Compensate, Tag Tag)>();
            _executionLock = new SemaphoreSlim(1, 1);

            Status = CompensatorStatus.Executing;
        }

        // This method is intentionally private.  whileExecuting is only applicable when compensation fails.
        private async Task CompensateAsync(Exception whileExecuting)
        {
            // short-circuit if compensated / failed to compensate
            if (Status == CompensatorStatus.Compensated)
                return;

            if (Status == CompensatorStatus.FailedToCompensate)
                throw new CompensatorStatusException(CompensatorStatus.FailedToCompensate);

            // acquire compensation lock
            await _compensationLock.WaitAsync().ConfigureAwait(false);

            try
            {
                // short-circuit if compensated
                if (Status == CompensatorStatus.Compensated)
                    return;

                if (Status == CompensatorStatus.FailedToCompensate)
                    throw new CompensatorStatusException(CompensatorStatus.FailedToCompensate);

                // set status
                Status = CompensatorStatus.Compensating;

                // acquire execution lock, i.e. make sure nothing is still executing
                await _executionLock.WaitAsync().ConfigureAwait(false);

                // compensate (work backwards through list)
                while (_compensations.Count > 0)
                {
                    // get last index
                    var lastIndex = _compensations.Count - 1;

                    // execute compensation if defined (i.e. not a tag)
                    if (_compensations[lastIndex].Compensate != null)
                        await _compensations[lastIndex].Compensate().ConfigureAwait(false);

                    // remove execution from list
                    _compensations.RemoveAt(lastIndex);
                }

                // set status
                Status = CompensatorStatus.Compensated;
            }
            catch (Exception whileCompensating)
            {
                // set status
                Status = CompensatorStatus.FailedToCompensate;

                // throw compensation exception
                throw (whileExecuting == null)
                    ? new CompensationException(whileCompensating)
                    : new CompensationException(whileCompensating, whileExecuting);
            }
            finally
            {
                // release locks
                _executionLock.Release();
                _compensationLock.Release();
            }
        }

        private int GetCompensateAtIndex(Tag tag)
        {
            // short-circuit if tag is null
            if (tag is null)
                return _compensations.Count;

            // find index of tag
            var tagIndex = 0;
            while (tagIndex < _compensations.Count) // TODO can compensations change?
            {
                if (_compensations[tagIndex].Tag == tag)
                    return tagIndex;
                tagIndex++;
            }

            throw new TagNotFoundException();
        }

        private void ValidateStatusIsExecuting()
        {
            if (Status != CompensatorStatus.Executing)
                throw new CompensatorStatusException(Status);
        }

        private void ValidateTag(Tag tag)
        {
            if (tag != null && !_compensations.Any(c => c.Tag == tag))
                throw new TagNotFoundException();
        }
    }
}