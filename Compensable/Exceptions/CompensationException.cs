namespace Compensable;

public class CompensationException(Exception whileCompensating, Exception whileExecuting) : Exception(
    message: whileExecuting is null
        ? $"While compensating: {whileCompensating?.Message}"
        : $"While executing: {whileExecuting.Message}{Environment.NewLine}While compensating: {whileCompensating?.Message}",
    innerException: whileExecuting ?? whileCompensating)
{
    /// <summary>
    /// The exception that was thrown while compensating.
    /// </summary>
    public Exception WhileCompensating { get; } = whileCompensating;

    /// <summary>
    /// The exception that was thrown while executing.
    /// </summary>
    public Exception WhileExecuting { get; } = whileExecuting;
}