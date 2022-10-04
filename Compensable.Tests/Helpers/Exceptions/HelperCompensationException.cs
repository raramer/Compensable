using System.Runtime.Serialization;

namespace Compensable.Tests.Helpers.Exceptions;

[Serializable]
public class HelperCompensationException : Exception
{
    public HelperCompensationException() : base(ExpectedMessages.CompensateFailed)
    {
    }

    public HelperCompensationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
