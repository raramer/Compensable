namespace Compensable.Tests.Helpers.Options;

public sealed record ItemEnumerationOptions(
    bool ExpectToBeCalled = true,
    bool ThrowsException = false)
{
    public bool ExpectSuccess => ExpectToBeCalled && !ThrowsException;

    public static ItemEnumerationOptions ShouldBeCalledAndSucceed
        => new ItemEnumerationOptions(ExpectToBeCalled: true, ThrowsException: false);

    public static ItemEnumerationOptions ShouldBeCalledAndThrowException
        => new ItemEnumerationOptions(ExpectToBeCalled: true, ThrowsException: true);

    public static ItemEnumerationOptions WouldSucceedButNotCalled
        => new ItemEnumerationOptions(ExpectToBeCalled: false, ThrowsException: false);

    public static ItemEnumerationOptions WouldThrowExceptionButNotCalled
        => new ItemEnumerationOptions(ExpectToBeCalled: false, ThrowsException: true);
}
