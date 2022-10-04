using System.Runtime.Serialization;

namespace Compensable.Tests.Helpers.Exceptions;

[Serializable]
public class HelperExecutionException : Exception
{
    public HelperExecutionException() : base(ExpectedMessages.ExecuteFailed)
    {
    }

    public HelperExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}