namespace Compensable.Tests.Helpers.Bases;

public abstract class CompensateHelperBase : HelperBase
{
    protected bool CompensationCalled { get; set; }
    
    protected DateTime? CompensationCalledAt { get; set; }
    
    protected CompensationOptions CompensationOptions { get; }

    protected CompensateHelperBase(CompensationOptions compensationOptions, string? label = null) 
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
