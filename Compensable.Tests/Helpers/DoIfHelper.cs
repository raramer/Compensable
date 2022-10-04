namespace Compensable.Tests.Helpers;

public class DoIfHelper : DoHelper
{
    public bool ExpectTestToBeCalled { get; }
    public bool TestCalled { get; private set; }
    public bool TestResult { get; }
    public bool ThrowOnTest { get; }

    public DoIfHelper(bool testResult, string? label = null, bool throwOnTest = false, bool throwOnExecute = false, bool throwOnCompensate = false, bool expectTestToBeCalled = true, bool expectExecutionToBeCalled = true, bool expectCompensationToBeCalled = false) : base(
        label: label,
        throwOnExecute: throwOnExecute,
        throwOnCompensate: throwOnCompensate,
        expectExecutionToBeCalled: expectTestToBeCalled && testResult && !throwOnTest && expectExecutionToBeCalled,
        expectCompensationToBeCalled: expectTestToBeCalled && testResult && !throwOnTest && expectCompensationToBeCalled)
    {
        ExpectTestToBeCalled = expectTestToBeCalled;
        TestResult = testResult;
        ThrowOnTest = throwOnTest;
    }

    public override void AssertHelper()
    {
        Assert.Equal(ExpectTestToBeCalled, TestCalled);
        base.AssertHelper();
    }

    public async Task<bool> TestAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        TestCalled = true;
        if (ThrowOnTest)
            throw new HelperTestException();
        return TestResult;
    }
}