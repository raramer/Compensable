using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class GetHelper : ExecuteCompensateHelperBase
{
    public Func<object, Task>? CompensateAsync { get; }

    public Func<Task<object>>? ExecuteAsync { get; }

    public object ExpectedExecuteResult { get; }
    
    private bool CompensationCalledWithResult { get; set; }

    public GetHelper(string? label = null)
        : this(Execution.ExpectToBeCalledAndSucceed, 
            Compensation.WouldSucceedButNotCalled, 
            label)
    {
    }

    public GetHelper(Execution.Options executionOptions, string? label = null)
        : this(executionOptions, 
            Compensation.WouldSucceedButNotCalled, 
            label)
    {
    }

    public GetHelper(Compensation.Options compensationOptions, string? label = null)
        : this(Execution.ExpectToBeCalledAndSucceed, 
            compensationOptions, 
            label)
    {
    }

    private GetHelper(Execution.Options executionOptions, Compensation.Options compensationOptions, string? label)
        : base(executionOptions, compensationOptions, label)
    {
        ExpectedExecuteResult = new object();

        ExecuteAsync = ExecutionOptions.IsNull ? null : _ExecuteAsync;
        CompensateAsync = CompensationOptions.IsNull ? null : _CompensateAsync;
    }

    public override void AssertHelper()
    {
        base.AssertHelper();
        Assert.Equal(CompensationOptions.ExpectToBeCalled, CompensationCalledWithResult);
    }

    public override async Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation)
    {
        if (CompensateAsync is null || actualCompensation is null)
            return false;

        var isExpectedCompensation = false;

        // store existing state
        var rollbackExecutionCalled = ExecutionCalled;
        var rollbackCompensationCalled = CompensationCalled;
        var rollbackCompensationCalledAt = CompensationCalledAt;
        var rollbackCompensationCalledWithResult = CompensationCalledWithResult;

        try
        {
            // reset state
            ExecutionCalled = false;
            CompensationCalled = false;
            CompensationCalledAt = null;
            CompensationCalledWithResult = false;

            // call actual compensation
            await actualCompensation().ConfigureAwait(false);
        }
        catch
        {
            // we only care if it was called
        }
        finally 
        { 
            // if called, then is expected compensation
            isExpectedCompensation = CompensationCalledWithResult;

            // restore state
            ExecutionCalled = rollbackExecutionCalled;
            CompensationCalled = rollbackCompensationCalled;
            CompensationCalledAt = rollbackCompensationCalledAt;
            CompensationCalledWithResult = rollbackCompensationCalledWithResult;
        }

        return isExpectedCompensation;
    }

    private async Task _CompensateAsync(object result)
    {
        await Task.Delay(1).ConfigureAwait(false);
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        CompensationCalledWithResult = Object.Equals(result, ExpectedExecuteResult);
        if (CompensationOptions.ThrowsException)
            throw new HelperCompensationException();
    }

    private async Task<object> _ExecuteAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        ExecutionCalled = true;
        if (ExecutionOptions.ThrowsException)
            throw new HelperExecutionException();
        return ExpectedExecuteResult;
    }
}