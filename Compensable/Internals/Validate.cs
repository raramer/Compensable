namespace Compensable;

internal static class Validate
{
    internal static void Compensation(Delegate compensation)
    {
        if (compensation is null)
            throw new ArgumentNullException(nameof(compensation));
    }

    internal static void Execution(Delegate execution)
    {
        if (execution is null)
            throw new ArgumentNullException(nameof(execution));
    }

    internal static void ExecutionCompensation(IExecutionCompensation executionCompensation)
    {
        if (executionCompensation is null)
            throw new ExecutionCompensationNullException();
    }

    internal static void Items<T>(IEnumerable<T> items)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
    }

    internal static void Test(Delegate test)
    {
        if (test is null)
            throw new ArgumentNullException(nameof(test));
    }
}
