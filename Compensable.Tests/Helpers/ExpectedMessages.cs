namespace Compensable.Tests.Helpers
{
    internal static class ExpectedMessages
    {
        internal const string CompensateFailed = "Compensate failed";

        internal const string ExecuteFailed = "Execute failed";

        internal const string TagNotFound = "Tag not found";

        internal const string TestFailed = "Test failed";

        internal static string WhileCompensating
            => $"While compensating: {CompensateFailed}";

        internal static string WhileExecutingWhileCompensating
            => $"While executing: {ExecuteFailed}{Environment.NewLine}While compensating: {CompensateFailed}";

        internal static string CompensatorStatusIs(CompensatorStatus compensatorStatus)
            => $"Compensator status is {compensatorStatus}";

        internal static string ValueConnotBeNull(string parameterName)
                    => new ArgumentNullException(parameterName).Message;
    }
}