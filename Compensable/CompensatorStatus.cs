namespace Compensable
{
    public enum CompensatorStatus
    {
        // TODO do we need additional statuses?  Ready, Idle, Committing, Committed, Disposed
        Executing = 0,
        FailedToExecute = 1,
        Compensating = 2,
        Compensated = 3,
        FailedToCompensate = 4,
    }
}
