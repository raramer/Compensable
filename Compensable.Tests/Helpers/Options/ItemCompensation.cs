namespace Compensable.Tests.Helpers.Options;

public static class ItemCompensation
{
    public sealed record Options(
        bool ExpectToBeCalled = false, 
        bool ThrowsException = false)
    {
        public static implicit operator Compensation.Options?(Options? _) 
            => _ is null ? null : new Compensation.Options(
                IsNull: false, 
                ExpectToBeCalled: _.ExpectToBeCalled, 
                ThrowsException: _.ThrowsException);
    }

    public static Options ShouldBeCalledAndSucceed
        => new Options(ExpectToBeCalled: true, ThrowsException: false);

    public static Options ShouldBeCalledAndThrowException
        => new Options(ExpectToBeCalled: true, ThrowsException: true);

    public static Options WouldSucceedButNotCalled
        => new Options(ExpectToBeCalled: false, ThrowsException: false);

    public static Options WouldThrowExceptionButNotCalled
        => new Options(ExpectToBeCalled: false, ThrowsException: true);
}
