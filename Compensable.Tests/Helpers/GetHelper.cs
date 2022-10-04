namespace Compensable.Tests.Helpers;

public class GetHelper : ExecuteCompensateHelperBase
{
    public bool CompensationCalledWithResult { get; protected set; }
    public object ExecuteResult { get; }

    public GetHelper(string? label = null, bool throwOnExecute = false, bool throwOnCompensate = false, bool expectExecutionToBeCalled = true, bool expectCompensationToBeCalled = false) : base(
        label: label,
        throwOnExecute: throwOnExecute,
        throwOnCompensate: throwOnCompensate,
        expectExecutionToBeCalled: expectExecutionToBeCalled,
        expectCompensationToBeCalled: expectCompensationToBeCalled)
    {
        ExecuteResult = new object();
    }

    public override void AssertHelper()
    {
        base.AssertHelper();
        Assert.Equal(ExpectCompensationToBeCalled, CompensationCalledWithResult);
    }

    public async Task CompensateAsync(object result)
    {
        await Task.Delay(1).ConfigureAwait(false);
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        CompensationCalledWithResult = Object.Equals(result, ExecuteResult);
        if (ThrowOnCompensate)
            throw new HelperCompensationException();
    }

    public async Task<object> ExecuteAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        ExecutionCalled = true;
        if (ThrowOnExecute)
            throw new HelperExecutionException();
        return ExecuteResult;
    }

    // The original CompensateAsync is wrapped in another lambda in the compensator stack. This hack allows us to test if the expected method and
    // actual method are the same by checking CompensationCalled.
    public async Task WhileCompensationCalledResetAsync(Func<Task> action)
    {
        var rollbackCompensationCalled = CompensationCalled;
        var rollbackCompensationCalledAt = CompensationCalledAt;
        var rollbackCompensationCalledWithResult = CompensationCalledWithResult;

        CompensationCalled = false;
        CompensationCalledAt = null;
        CompensationCalledWithResult = false;

        try
        {
            await action().ConfigureAwait(false);
        }
        finally
        {
            CompensationCalled = rollbackCompensationCalled;
            CompensationCalledAt = rollbackCompensationCalledAt;
            CompensationCalledWithResult = rollbackCompensationCalledWithResult;
        }
    }
}