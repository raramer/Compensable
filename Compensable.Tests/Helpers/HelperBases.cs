namespace Compensable.Tests.Helpers;

public abstract class CompensateHelperBase : HelperBase
{
    public bool CompensationCalled { get; protected set; }
    public DateTime? CompensationCalledAt { get; protected set; }
    public bool ExpectCompensationToBeCalled { get; }
    public bool ThrowOnCompensate { get; }

    protected CompensateHelperBase(string? label, bool throwOnCompensate, bool expectCompensationToBeCalled) : base(label)
    {
        ExpectCompensationToBeCalled = expectCompensationToBeCalled;
        ThrowOnCompensate = throwOnCompensate;
    }

    public override void AssertHelper()
    {
        Assert.Equal(ExpectCompensationToBeCalled, CompensationCalled);
        if (ExpectCompensationToBeCalled)
        {
            Assert.NotNull(CompensationCalledAt);
        }
        else
        {
            Assert.Null(CompensationCalledAt);
        }
    }
}

public abstract class ExecuteCompensateHelperBase : CompensateHelperBase
{
    public bool ExecutionCalled { get; protected set; }
    public bool ExpectExecutionToBeCalled { get; }
    public bool ThrowOnExecute { get; }

    protected ExecuteCompensateHelperBase(string? label, bool throwOnExecute, bool throwOnCompensate, bool expectExecutionToBeCalled, bool expectCompensationToBeCalled) : base(
        label: label,
        throwOnCompensate: throwOnCompensate,
        expectCompensationToBeCalled: expectExecutionToBeCalled && !throwOnExecute && expectCompensationToBeCalled)
    {
        ExpectExecutionToBeCalled = expectExecutionToBeCalled;
        ThrowOnExecute = throwOnExecute;
    }

    public override void AssertHelper()
    {
        Assert.Equal(ExpectExecutionToBeCalled, ExecutionCalled);
        base.AssertHelper();
    }
}

public abstract class HelperBase
{
    #pragma warning disable IDE1006
    public string _Label_ { get; }
    #pragma warning restore IDE1006

    protected HelperBase(string? label)
    {
        _Label_ = string.IsNullOrWhiteSpace(label)
            ? $"{GetType().Name} {Guid.NewGuid():n}"
            : label;
    }

    public abstract void AssertHelper();
}