using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class ItemHelper : ExecuteCompensateHelperBase
{
    public object ExpectedItem { get; }

    public bool ExpectInCompensationStack => !CompensationOptions.ExpectToBeCalled || CompensationOptions.ThrowsException;

    public object Item
    {
        get
        {
            ItemEnumerated = true;
            if (EnumerationOptions.ThrowsException)
                throw new HelperItemException();
            return ExpectedItem;
        }
    }

    private bool CompensationCalledWithItem { get; set; }

    private ItemEnumeration.Options EnumerationOptions { get; }
    private bool ExecutionCalledWithItem { get; set; }

    private bool ItemEnumerated { get; set; }

    public ItemHelper(string? label = null)
        : this(ItemEnumeration.ShouldBeCalledAndSucceed,
              ItemExecution.ShouldBeCalledAndSucceed,
              ItemCompensation.WouldSucceedButNotCalled,
              label)
    {
    }

    public ItemHelper(ItemEnumeration.Options enumerationOptions, string? label = null)
        : this(enumerationOptions,
              executionOptions: enumerationOptions.ExpectSuccess ? ItemExecution.ShouldBeCalledAndSucceed : ItemExecution.WouldSucceedButNotCalled,
              ItemCompensation.WouldSucceedButNotCalled,
              label)
    {
    }

    public ItemHelper(ItemExecution.Options executionOptions, string? label = null)
        : this(ItemEnumeration.ShouldBeCalledAndSucceed,
              executionOptions,
              ItemCompensation.WouldSucceedButNotCalled,
              label)
    {
    }

    public ItemHelper(ItemCompensation.Options compensationOptions, string? label = null)
        : this(ItemEnumeration.ShouldBeCalledAndSucceed,
              ItemExecution.ShouldBeCalledAndSucceed,
              compensationOptions,
              label)
    {
    }

    private ItemHelper(ItemEnumeration.Options enumerationOptions, ItemExecution.Options executionOptions, ItemCompensation.Options compensationOptions, string? label)
        : base(executionOptions, compensationOptions, label)
    {
        ExpectedItem = new object();

        EnumerationOptions = enumerationOptions;
    }

    public override void AssertHelper()
    {
        Assert.Equal(EnumerationOptions.ExpectToBeCalled, ItemEnumerated);
        base.AssertHelper();
        Assert.Equal(ExecutionOptions.ExpectToBeCalled, ExecutionCalledWithItem);
        Assert.Equal(CompensationOptions.ExpectToBeCalled, CompensationCalledWithItem);
    }

    public async Task CompensateAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        CompensationCalledWithItem = Object.Equals(item, ExpectedItem);
        if (CompensationOptions.ThrowsException)
            throw new HelperCompensationException();
    }

    public async Task ExecuteAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        ExecutionCalled = true;
        ExecutionCalledWithItem = Object.Equals(item, ExpectedItem);
        if (ExecutionOptions.ThrowsException)
            throw new HelperExecutionException();
    }

    public override async Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation)
    {
        if (actualCompensation is null)
            return false;

        var isExpectedCompensation = false;

        // store existing state
        var rollbackItemEnumerated = ItemEnumerated;
        var rollbackExecutionCalled = ExecutionCalled;
        var rollbackExecutionCalledWithItem = ExecutionCalledWithItem;
        var rollbackCompensationCalled = CompensationCalled;
        var rollbackCompensationCalledAt = CompensationCalledAt;
        var rollbackCompensationCalledWithItem = CompensationCalledWithItem;

        try
        {
            // reset state
            ItemEnumerated = false;
            ExecutionCalled = false;
            ExecutionCalledWithItem = false;
            CompensationCalled = false;
            CompensationCalledAt = null;
            CompensationCalledWithItem = false;

            // call actual compensation
            await actualCompensation().ConfigureAwait(false);
        }
        catch
        {
            // we only care if it was called
        }
        finally
        {
            // if called, then is expected compensation
            isExpectedCompensation = CompensationCalledWithItem;

            // restore state
            ItemEnumerated = rollbackItemEnumerated;
            ExecutionCalled = rollbackExecutionCalled;
            ExecutionCalledWithItem = rollbackExecutionCalledWithItem;
            CompensationCalled = rollbackCompensationCalled;
            CompensationCalledAt = rollbackCompensationCalledAt;
            CompensationCalledWithItem = rollbackCompensationCalledWithItem;
        }

        return isExpectedCompensation;
    }
}