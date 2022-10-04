namespace Compensable.Tests.Helpers
{
    public class ItemHelper : ExecuteCompensateHelperBase
    {
        private readonly object _item;

        public bool CompensationCalledWithItem { get; protected set; }
        public bool ExecutionCalledWithItem { get; protected set; }
        public bool ExpectItemToBeCalled { get; }
        public object Item
        {
            get
            {
                ItemCalled = true;
                if (ThrowOnItem)
                    throw new HelperItemException();
                return _item;
            }
        }
        public bool ItemCalled { get; private set; }
        public bool ThrowOnItem { get; }

        public ItemHelper(string? label = null, bool throwOnItem = false, bool throwOnExecute = false, bool throwOnCompensate = false, bool expectItemToBeCalled = true, bool expectExecutionToBeCalled = true, bool expectCompensationToBeCalled = false) : base(
            label: label,
            throwOnExecute: throwOnExecute,
            throwOnCompensate: throwOnCompensate,
            expectExecutionToBeCalled: expectItemToBeCalled && !throwOnItem && expectExecutionToBeCalled,
            expectCompensationToBeCalled: expectItemToBeCalled && !throwOnItem && expectCompensationToBeCalled)
        {
            _item = new object();
            ExpectItemToBeCalled = expectItemToBeCalled;
            ThrowOnItem = throwOnItem;
        }

        public override void AssertHelper()
        {
            Assert.Equal(ExpectItemToBeCalled, ItemCalled);
            base.AssertHelper();
            Assert.Equal(ExpectExecutionToBeCalled, ExecutionCalledWithItem);
            Assert.Equal(ExpectCompensationToBeCalled, CompensationCalledWithItem);
        }

        public async Task CompensateAsync(object item)
        {
            await Task.Delay(1).ConfigureAwait(false);
            CompensationCalled = true;
            CompensationCalledAt = DateTime.UtcNow;
            CompensationCalledWithItem = Object.Equals(item, _item);
            if (ThrowOnCompensate)
                throw new HelperCompensationException();
        }

        public async Task ExecuteAsync(object item)
        {
            await Task.Delay(1).ConfigureAwait(false);
            ExecutionCalled = true;
            ExecutionCalledWithItem = Object.Equals(item, _item);
            if (ThrowOnExecute)
                throw new HelperExecutionException();
        }

        // The original CompensateAsync is wrapped in another lambda in the compensator stack. This hack allows us to test if the expected method and
        // actual method are the same by checking CompensationCalled.
        public async Task WhileCompensationCalledResetAsync(Func<Task> action)
        {
            var rollbackCompensationCalled = CompensationCalled;
            var rollbackCompensationCalledAt = CompensationCalledAt;
            var rollbackCompensationCalledWithItem = CompensationCalledWithItem;

            CompensationCalled = false;
            CompensationCalledAt = null;
            CompensationCalledWithItem = false;

            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                CompensationCalled = rollbackCompensationCalled;
                CompensationCalledAt = rollbackCompensationCalledAt;
                CompensationCalledWithItem = rollbackCompensationCalledWithItem;
            }
        }
    }
}