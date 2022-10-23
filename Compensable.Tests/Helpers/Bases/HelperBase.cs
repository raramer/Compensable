namespace Compensable.Tests.Helpers.Bases;

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

    public abstract Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation);
}