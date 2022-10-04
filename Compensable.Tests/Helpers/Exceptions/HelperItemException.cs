using System.Runtime.Serialization;

namespace Compensable.Tests.Helpers.Exceptions;

[Serializable]
public class HelperItemException : Exception
{
    public HelperItemException() : base(ExpectedMessages.ItemFailed)
    {
    }

    public HelperItemException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}