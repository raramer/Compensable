using System.Xml.Serialization;

namespace Compensable.Tests.Helpers.Extensions;

internal static class ExceptionExtensions
{
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
