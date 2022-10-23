namespace Compensable.Tests.Helpers.Bases;

public abstract class ExecuteCompensateHelperBase : CompensateHelperBase
{
    protected bool ExecutionCalled { get; set; }

    protected Execution.Options ExecutionOptions { get; }

    protected ExecuteCompensateHelperBase(Execution.Options executionOptions, Compensation.Options compensationOptions, string? label) 
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
