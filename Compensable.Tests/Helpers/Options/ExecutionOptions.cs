namespace Compensable.Tests.Helpers.Options;

public sealed record ExecutionOptions(
    bool IsNull = false, 
    bool ExpectToBeCalled = true, 
    bool ThrowsException = false)
{
    public bool ExpectSuccess => !IsNull && ExpectToBeCalled && !ThrowsException;

    public static ExecutionOptions Null 
        => new ExecutionOptions(IsNull: true, ExpectToBeCalled: false, ThrowsException: false);

    public static ExecutionOptions ExpectToBeCalledAndSucceed 
        => new ExecutionOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: false);
    
    public static ExecutionOptions ExpectToBeCalledAndThrowException 
        => new ExecutionOptions(IsNull: false, ExpectToBeCalled: true, ThrowsException: true);
    
    public static ExecutionOptions WouldSucceedButNotCalled 
        => new ExecutionOptions(IsNull: false, ExpectToBeCalled: false, ThrowsException: false);
    
    public static ExecutionOptions WouldThrowExceptionButNotCalled 
        => new ExecutionOptions(IsNull: false, ExpectToBeCalled: false, ThrowsException: true);
}
