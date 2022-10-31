using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class DoHelper : ExecuteCompensateHelperBase
{
    public Action? Compensate { get; }
    public Func<Task>? CompensateAsync { get; }

    public Action? Execute { get; }
    public Func<Task>? ExecuteAsync { get; }

    public DoHelper(string? label = null)
        : this(ExecutionOptions.ExpectToBeCalledAndSucceed,
            CompensationOptions.WouldSucceedButNotCalled,
            label)
    {
    }

    public DoHelper(ExecutionOptions executionOptions, string? label = null)
        : this(executionOptions,
            CompensationOptions.WouldSucceedButNotCalled,
            label)
    {
    }

    public DoHelper(CompensationOptions compensationOptions, string? label = null)
        : this(ExecutionOptions.ExpectToBeCalledAndSucceed,
            compensationOptions,
            label)
    {
    }

    protected DoHelper(ExecutionOptions executionOptions, CompensationOptions compensationOptions, string? label)
        : base(executionOptions, compensationOptions, label)
    {
        if (!ExecutionOptions.IsNull)
        {
            Execute = _Execute;
            ExecuteAsync = _ExecuteAsync;
        }

        if (!CompensationOptions.IsNull)
        {
            Compensate = _Compensate;
            CompensateAsync = _CompensateAsync;
        }
    }

    public override Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation)
    {
        return Task.FromResult(CompensateAsync is not null && actualCompensation is not null && CompensateAsync == actualCompensation);
    }

    private void _Compensate()
    {
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        if (CompensationOptions.ThrowsException)
            throw new HelperCompensationException();
    }
    private async Task _CompensateAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        _Compensate();
    }


    private void _Execute()
    {
        ExecutionCalled = true;
        if (ExecutionOptions.ThrowsException)
            throw new HelperExecutionException();
    }

    private async Task _ExecuteAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        _Execute();
    }
}