namespace Compensable.Tests.Helpers.Bases;

public abstract class CompensateHelperBase : HelperBase
{
    protected bool CompensationCalled { get; set; }
    
    protected DateTime? CompensationCalledAt { get; set; }
    
    protected Compensation.Options CompensationOptions { get; }

    protected CompensateHelperBase(Compensation.Options compensationOptions, string? label = null) 
        : base(label)
    {
        CompensationOptions = compensationOptions;
    }

    public override void AssertHelper()
    {
        Assert.Equal(CompensationOptions.ExpectToBeCalled, CompensationCalled);
        if (CompensationOptions.ExpectToBeCalled)
        {
            Assert.NotNull(CompensationCalledAt);
        }
        else
        {
            Assert.Null(CompensationCalledAt);
        }
    }
}
