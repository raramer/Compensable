﻿namespace Compensable.Tests.Extensions;

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
