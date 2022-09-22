namespace Compensable.Tests.Helpers
{
    public abstract class CompensateHelperBase
    {
        protected bool _expectCompensationToBeCalled;

        public bool CompensationCalled { get; protected set; }
        public DateTime? CompensationCalledAt { get; protected set; }
        public Exception? CompensationException { get; }
        public string Label { get; }

        protected CompensateHelperBase(string label, bool throwOnCompensate, bool expectCompensationToBeCalled)
        {
            Label = string.IsNullOrWhiteSpace(label)
                ? $"{GetType().Name} {Guid.NewGuid().ToString("n")}"
                : label;

            if (throwOnCompensate)
                CompensationException = new HelperCompensationException(ExpectedMessages.CompensateFailed);

            _expectCompensationToBeCalled = expectCompensationToBeCalled;
        }

        public virtual void AssertHelper()
        {
            Assert.Equal(_expectCompensationToBeCalled, CompensationCalled);
            if (_expectCompensationToBeCalled)
            {
                Assert.NotNull(CompensationCalledAt);
            }
            else
            {
                Assert.Null(CompensationCalledAt);
            }
        }
    }
    public abstract class ExecuteCompensateHelperBase : CompensateHelperBase
    {
        private bool _expectExecutionToBeCalled;

        public bool ExecutionCalled { get; protected set; }
        public Exception? ExecutionException { get; }

        protected ExecuteCompensateHelperBase(string label, bool throwOnExecute, bool throwOnCompensate, bool expectExecutionToBeCalled, bool expectCompensationToBeCalled)
            : base(
                  label: label, 
                  throwOnCompensate: throwOnCompensate, 
                  expectCompensationToBeCalled: expectExecutionToBeCalled && !throwOnExecute && expectCompensationToBeCalled)
        {
            if (throwOnExecute)
                ExecutionException = new HelperExecutionException(ExpectedMessages.ExecuteFailed);

            _expectExecutionToBeCalled = expectExecutionToBeCalled;
        }

        public override void AssertHelper()
        {
            Assert.Equal(_expectExecutionToBeCalled, ExecutionCalled);
            base.AssertHelper();
        }
    }
}