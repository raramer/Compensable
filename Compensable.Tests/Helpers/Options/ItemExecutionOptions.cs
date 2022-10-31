namespace Compensable.Tests.Helpers.Options;

public sealed record ItemExecutionOptions(
    bool ExpectToBeCalled = true,
    bool ThrowsException = false)
{
    public bool ExpectSuccess => ExpectToBeCalled && !ThrowsException;

    public static implicit operator ExecutionOptions?(ItemExecutionOptions? _)
        => _ is null ? null : new ExecutionOptions(
            IsNull: false, 
            ExpectToBeCalled: _.ExpectToBeCalled, 
            ThrowsException: _.ThrowsException);

    public static ItemExecutionOptions ShouldBeCalledAndSucceed
        => new ItemExecutionOptions(ExpectToBeCalled: true, ThrowsException: false);

    public static ItemExecutionOptions ShouldBeCalledAndThrowException
        => new ItemExecutionOptions(ExpectToBeCalled: true, ThrowsException: true);

    public static ItemExecutionOptions WouldSucceedButNotCalled
        => new ItemExecutionOptions(ExpectToBeCalled: false, ThrowsException: false);

    public static ItemExecutionOptions WouldThrowExceptionButNotCalled
        => new ItemExecutionOptions(ExpectToBeCalled: false, ThrowsException: true);
}
