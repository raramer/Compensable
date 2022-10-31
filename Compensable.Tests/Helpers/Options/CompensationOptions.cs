namespace Compensable.Tests.Helpers.Options;

public sealed record CompensationOptions(
    bool IsNull = false, 
    bool ExpectToBeCalled = false, 
    bool ThrowsException = false)
{
    public static CompensationOptions Null
        => new CompensationOptions(IsNull: true, ExpectToBeCalled: false, ThrowsException: false);

    public static CompensationOptions ExpectToBeCalledAndSucceed
        => new CompensationOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: false);

    public static CompensationOptions ExpectToBeCalledAndThrowException
        => new CompensationOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: true);

    public static CompensationOptions WouldSucceedButNotCalled
        => new CompensationOptions(IsNull: false, ExpectToBeCalled: false, ThrowsException: false);

    public static CompensationOptions WouldThrowExceptionButNotCalled
        => new CompensationOptions(IsNull: false, ExpectToBeCalled: false, ThrowsException: true);
}
