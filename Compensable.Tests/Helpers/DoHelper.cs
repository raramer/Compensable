using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class DoHelper : ExecuteCompensateHelperBase
{
    public Func<Task>? CompensateAsync { get; }

    public Func<Task>? ExecuteAsync { get; }

    public DoHelper(string? label = null)
        : this(Execution.ExpectToBeCalledAndSucceed, 
            Compensation.WouldSucceedButNotCalled, 
            label)
    {
    }

    public DoHelper(Execution.Options executionOptions, string? label = null)
        : this(executionOptions, 
            Compensation.WouldSucceedButNotCalled, 
            label)
    {
    }

    public DoHelper(Compensation.Options compensationOptions, string? label = null)
        : this(Execution.ExpectToBeCalledAndSucceed, 
            compensationOptions, 
            label)
    {
    }

    protected DoHelper(Execution.Options executionOptions, Compensation.Options compensationOptions, string? label) 
        : base(executionOptions, compensationOptions, label)
    {
        ExecuteAsync = ExecutionOptions.IsNull ? null : _ExecuteAsync;
        CompensateAsync = CompensationOptions.IsNull ? null : _CompensateAsync;
    }

    public override Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation)
    {
        return Task.FromResult(CompensateAsync is not null && actualCompensation is not null && CompensateAsync == actualCompensation);
    }

    private async Task _CompensateAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        if (CompensationOptions.ThrowsException)
            throw new HelperCompensationException();
    }

    private async Task _ExecuteAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        ExecutionCalled = true;
        if (ExecutionOptions.ThrowsException)
            throw new HelperExecutionException();
    }
}