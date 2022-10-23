namespace Compensable.Tests.Helpers.Options;

public static class ItemExecution
{
    public sealed record Options(
        bool ExpectToBeCalled = true,
        bool ThrowsException = false)
    {
        public bool ExpectSuccess => ExpectToBeCalled && !ThrowsException;

        public static implicit operator Execution.Options?(Options? _)
            => _ is null ? null : new Execution.Options(
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
