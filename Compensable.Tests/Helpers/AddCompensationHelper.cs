using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class AddCompensationHelper : CompensateHelperBase
{
    public Func<Task>? CompensateAsync { get; }

    public AddCompensationHelper(string? label = null) 
        : this(Compensation.WouldSucceedButNotCalled, label)
    {
    } 

    public AddCompensationHelper(Compensation.Options compensation, string? label = null) 
        : base(compensation, label)
    {
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
}