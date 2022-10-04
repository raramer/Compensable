using System.Runtime.Serialization;

namespace Compensable.Tests.Helpers.Exceptions;

[Serializable]
internal class HelperTestException : Exception
{
    public HelperTestException() : base(ExpectedMessages.TestFailed)
    {
    }
    public HelperTestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
