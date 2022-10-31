namespace Compensable.Tests.Helpers.Options;

public sealed record ItemCompensationOptions(
    bool ExpectToBeCalled = false, 
    bool ThrowsException = false)
{
    public static implicit operator CompensationOptions?(ItemCompensationOptions? _) 
        => _ is null ? null : new CompensationOptions(
            IsNull: false, 
            ExpectToBeCalled: _.ExpectToBeCalled, 
            ThrowsException: _.ThrowsException);

    public static ItemCompensationOptions ShouldBeCalledAndSucceed
        => new ItemCompensationOptions(ExpectToBeCalled: true, ThrowsException: false);

    public static ItemCompensationOptions ShouldBeCalledAndThrowException
        => new ItemCompensationOptions(ExpectToBeCalled: true, ThrowsException: true);

    public static ItemCompensationOptions WouldSucceedButNotCalled
        => new ItemCompensationOptions(ExpectToBeCalled: false, ThrowsException: false);

    public static ItemCompensationOptions WouldThrowExceptionButNotCalled
        => new ItemCompensationOptions(ExpectToBeCalled: false, ThrowsException: true);
}
