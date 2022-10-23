namespace Compensable.Tests;

internal static class ExpectedMessages
{
    internal const string CompensateFailed = "Compensate failed";

    internal const string ExecuteFailed = "Execute failed";

    internal const string ItemFailed = "Item failed";

    internal const string TagNotFound = "Tag not found";

    internal const string TestFailed = "Test failed";

    internal static string WhileCompensating(string whileCompensatingMessage = CompensateFailed)
        => $"While compensating: {whileCompensatingMessage}";

    internal static string WhileExecutingWhileCompensating(string whileExecutingMessage = ExecuteFailed, string whileCompensatingMessage = CompensateFailed)
        => $"While executing: {whileExecutingMessage}{Environment.NewLine}While compensating: {whileCompensatingMessage}";

    internal static string CompensatorStatusIs(CompensatorStatus compensatorStatus)
        => $"Compensator status is {compensatorStatus}";

    internal static string ValueCannotBeNull(string parameterName)
        => new ArgumentNullException(parameterName).Message;
}