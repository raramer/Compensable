using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class AddCompensationHelper : CompensateHelperBase
{
    public Action? Compensate { get; }

    public Func<Task>? CompensateAsync { get; }

    public AddCompensationHelper(string? label = null) 
        : this(CompensationOptions.WouldSucceedButNotCalled, label)
    {
    } 

    public AddCompensationHelper(CompensationOptions compensationOptions, string? label = null) 
        : base(compensationOptions, label)
    {
        if (!CompensationOptions.IsNull)
        {
            Compensate = _Compensate;
            CompensateAsync = _CompensateAsync;
        }
    }

    public override bool IsExpectedCompensation(Action actualCompensation)
    {
        return Compensate is not null && actualCompensation is not null && Compensate == actualCompensation;
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
}