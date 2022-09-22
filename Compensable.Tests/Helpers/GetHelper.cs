namespace Compensable.Tests.Helpers
{
    public class GetHelper : ExecuteCompensateHelperBase
    {
        public bool CompensationCalledWithResult { get; protected set; }
        public object ExecuteResult { get; }

        public GetHelper(string label = null, bool throwOnExecute = false, bool throwOnCompensate = false, bool expectExecutionToBeCalled = true, bool expectCompensationToBeCalled = false)
            : base(
                  label: label, 
                  throwOnExecute: throwOnExecute, 
                  throwOnCompensate: throwOnCompensate, 
                  expectExecutionToBeCalled: expectExecutionToBeCalled, 
                  expectCompensationToBeCalled: expectExecutionToBeCalled && !throwOnExecute && expectCompensationToBeCalled)
        {
            ExecuteResult = new object();
        }

        public override void AssertHelper()
        {
            base.AssertHelper();
            Assert.Equal(_expectCompensationToBeCalled, CompensationCalledWithResult);
        }

        public async Task CompensateAsync(object result)
        {
            await Task.Delay(1).ConfigureAwait(false);
            CompensationCalled = true;
            CompensationCalledAt = DateTime.UtcNow;
            CompensationCalledWithResult = Object.Equals(result, ExecuteResult);
            if (CompensationException != null)
                throw CompensationException;
        }

        public async Task<object> ExecuteAsync()
        {
            await Task.Delay(1).ConfigureAwait(false);
            ExecutionCalled = true;
            if (ExecutionException != null)
                throw ExecutionException;
            return ExecuteResult;
        }

        // The original CompensateAsync is wrapped in another lambda in the compensator stack. This hack allows us to test if the expected method and
        // actual method are the same by checking CompensationCalled.
        public GetHelper ResetCompensationCalled()
        {
            CompensationCalled = false;
            return this;
        }
    }
}