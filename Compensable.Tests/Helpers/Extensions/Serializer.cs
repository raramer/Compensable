using System.Runtime.Serialization.Formatters.Binary;

namespace Compensable.Tests.Helpers.Extensions;

internal static class ExceptionExtensions
{
    internal static TException SerializeAndDeserialize<TException>(this TException exception)
        where TException : Exception
    {
        var formatter = new BinaryFormatter();

        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, exception);
            stream.Seek(0, 0);
            return (TException)formatter.Deserialize(stream);
        }
    }

    internal static TException ThrowAndCatch<TException>(this TException exception)
        where TException : Exception
    {
        try
        {
            throw exception;
        }
        catch (TException thrownAndCaughtException)
        {
            return thrownAndCaughtException;
        }
    }    
}
