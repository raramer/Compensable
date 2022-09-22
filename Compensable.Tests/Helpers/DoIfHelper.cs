namespace Compensable.Tests.Helpers
{
    public class DoIfHelper : DoHelper
    {
        private bool _expectTestToBeCalled;

        public bool TestCalled { get; private set; }
        public Exception TestException { get; }
        public bool TestResult { get; }

        public DoIfHelper(bool testResult, string label = null, bool throwOnTest = false, bool throwOnExecute = false, bool throwOnCompensate = false, bool expectTestToBeCalled = true, bool expectExecutionToBeCalled = true, bool expectCompensationToBeCalled = false)
            : base(
                  label: label, 
                  throwOnExecute: throwOnExecute, 
                  throwOnCompensate: throwOnCompensate,
                  expectExecutionToBeCalled: expectTestToBeCalled && testResult && !throwOnTest && expectExecutionToBeCalled, 
                  expectCompensationToBeCalled: expectTestToBeCalled && testResult && !throwOnTest && expectCompensationToBeCalled)
        {
            TestResult = testResult;

            if (throwOnTest)
                TestException = new HelperTestException(ExpectedMessages.TestFailed);

            _expectTestToBeCalled = expectTestToBeCalled;
        }

        public override void AssertHelper()
        {
            Assert.Equal(_expectTestToBeCalled, TestCalled);
            base.AssertHelper();
        }

        public async Task<bool> TestAsync()
        {
            await Task.Delay(1).ConfigureAwait(false);
            TestCalled = true;
            if (TestException != null)
                throw TestException;
            return TestResult;
        }
    }
}