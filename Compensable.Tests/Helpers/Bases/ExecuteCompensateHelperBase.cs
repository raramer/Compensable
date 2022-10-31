namespace Compensable.Tests.Helpers.Bases;

public abstract class ExecuteCompensateHelperBase : CompensateHelperBase
{
    protected bool ExecutionCalled { get; set; }

    protected ExecutionOptions ExecutionOptions { get; }

    protected ExecuteCompensateHelperBase(ExecutionOptions executionOptions, CompensationOptions compensationOptions, string? label) 
        : base(compensationOptions, label)
    {
        ExecutionOptions = executionOptions;
    }

    public override void AssertHelper()
    {
        Assert.Equal(ExecutionOptions.ExpectToBeCalled, ExecutionCalled);
        base.AssertHelper();
    }
}
