namespace Compensable.Tests.Helpers.Options;

public sealed record TestOptions(
    bool IsNull = false,
    bool ExpectToBeCalled = true,
    bool ThrowsException = false,
    bool Result = true)
{
    public bool ExpectTrueResult => !IsNull && ExpectToBeCalled && !ThrowsException && Result;

    public static TestOptions Null
        => new TestOptions(IsNull: true, ExpectToBeCalled: false, ThrowsException: false, Result: false);

    public static TestOptions ShouldBeCalledAndReturnFalse
        => new TestOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: false, Result: false);

    public static TestOptions ShouldBeCalledAndReturnTrue
        => new TestOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: false, Result: true);

    public static TestOptions ShouldBeCalledAndThrowException
        => new TestOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: true, Result: false);

    public static TestOptions WouldSucceedButNotCalled
        => new TestOptions(IsNull: false, ExpectToBeCalled: false, ThrowsException: false, Result: false);
}