namespace Compensable.Tests.Helpers.Options;

public static class Test
{
    public sealed record Options(
        bool IsNull = false,
        bool ExpectToBeCalled = true,
        bool ThrowsException = false,
        bool Result = true)
    {
        public bool ExpectTrueResult => !IsNull && ExpectToBeCalled && !ThrowsException && Result;
    }

    public static Options IsNull
        => new Options(IsNull: true, ExpectToBeCalled: false, ThrowsException: false, Result: false);

    public static Options ShouldBeCalledAndReturnFalse
        => new Options(IsNull: false, ExpectToBeCalled: true, ThrowsException: false, Result: false);

    public static Options ShouldBeCalledAndReturnTrue
        => new Options(IsNull: false, ExpectToBeCalled: true, ThrowsException: false, Result: true);

    public static Options ShouldBeCalledAndThrowException
        => new Options(IsNull: false, ExpectToBeCalled: true, ThrowsException: true, Result: false);

    public static Options WouldSucceedButNotCalled
        => new Options(IsNull: false, ExpectToBeCalled: false, ThrowsException: false, Result: false);
}