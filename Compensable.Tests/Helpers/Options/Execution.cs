namespace Compensable.Tests.Helpers.Options;

public static class Execution
{
    public sealed record Options(
        bool IsNull = false, 
        bool ExpectToBeCalled = true, 
        bool ThrowsException = false)
    {
        public bool ExpectSuccess => !IsNull && ExpectToBeCalled && !ThrowsException;
    }

    public static Options IsNull 
        => new Options(IsNull: true, ExpectToBeCalled: false, ThrowsException: false);

    public static Options ExpectToBeCalledAndSucceed 
        => new Options(IsNull: false, ExpectToBeCalled: true, ThrowsException: false);
    
    public static Options ExpectToBeCalledAndThrowException 
        => new Options(IsNull: false, ExpectToBeCalled: true, ThrowsException: true);
    
    public static Options WouldSucceedButNotCalled 
        => new Options(IsNull: false, ExpectToBeCalled: false, ThrowsException: false);
    
    public static Options WouldThrowExceptionButNotCalled 
        => new Options(IsNull: false, ExpectToBeCalled: false, ThrowsException: true);
}
