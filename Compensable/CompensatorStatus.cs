namespace Compensable
{
    // TODO do we need additional statuses?  Ready, Idle, Committing, Committed, Disposed
    public enum CompensatorStatus
    {
        /// <summary>
        /// The compensator is ready to accept executions.
        /// </summary>
        Executing = 0,

        /// <summary>
        /// The compensator failed while running an execution and will soon begin compensating.
        /// </summary>
        FailedToExecute = 1,

        /// <summary>
        /// The compensator is running the compensations in the compensation stack.
        /// </summary>
        Compensating = 2,

        /// <summary>
        /// The compensator successfully ran all compensations in the compensation stack.
        /// </summary>
        Compensated = 3,

        /// <summary>
        /// The compensator failed to run a compensation in the compensation stack.
        /// </summary>
        FailedToCompensate = 4,
    }
}
