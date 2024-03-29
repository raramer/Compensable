﻿using Compensable.Tests.Helpers.Bases;

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

    private ItemEnumerationOptions EnumerationOptions { get; }
    private bool ExecutionCalledWithItem { get; set; }

    private bool ItemEnumerated { get; set; }

    public ItemHelper(string? label = null)
        : this(ItemEnumerationOptions.ShouldBeCalledAndSucceed,
              ItemExecutionOptions.ShouldBeCalledAndSucceed,
              ItemCompensationOptions.WouldSucceedButNotCalled,
              label)
    {
    }

    public ItemHelper(ItemEnumerationOptions enumerationOptions, string? label = null)
        : this(enumerationOptions,
              executionOptions: enumerationOptions.ExpectSuccess ? ItemExecutionOptions.ShouldBeCalledAndSucceed : ItemExecutionOptions.WouldSucceedButNotCalled,
              ItemCompensationOptions.WouldSucceedButNotCalled,
              label)
    {
    }

    public ItemHelper(ItemExecutionOptions executionOptions, string? label = null)
        : this(ItemEnumerationOptions.ShouldBeCalledAndSucceed,
              executionOptions,
              ItemCompensationOptions.WouldSucceedButNotCalled,
              label)
    {
    }

    public ItemHelper(ItemCompensationOptions compensationOptions, string? label = null)
        : this(ItemEnumerationOptions.ShouldBeCalledAndSucceed,
              ItemExecutionOptions.ShouldBeCalledAndSucceed,
              compensationOptions,
              label)
    {
    }

    #pragma warning disable CS8604
    private ItemHelper(ItemEnumerationOptions enumerationOptions, ItemExecutionOptions executionOptions, ItemCompensationOptions compensationOptions, string? label)
        : base(executionOptions, compensationOptions, label)
    {
        ExpectedItem = new object();

        EnumerationOptions = enumerationOptions;
    }
    #pragma warning restore CS8604

    public override void AssertHelper()
    {
        Assert.Equal(EnumerationOptions.ExpectToBeCalled, ItemEnumerated);
        base.AssertHelper();
        Assert.Equal(ExecutionOptions.ExpectToBeCalled, ExecutionCalledWithItem);
        Assert.Equal(CompensationOptions.ExpectToBeCalled, CompensationCalledWithItem);
    }

    public void Compensate(object item)
    {
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        CompensationCalledWithItem = Object.Equals(item, ExpectedItem);
        if (CompensationOptions.ThrowsException)
            throw new HelperCompensationException();
    }

    public void Execute(object item)
    {
        ExecutionCalled = true;
        ExecutionCalledWithItem = Object.Equals(item, ExpectedItem);
        if (ExecutionOptions.ThrowsException)
            throw new HelperExecutionException();
    }

    public override bool IsExpectedCompensation(Action actualCompensation)
    {
        return IsExpectedCompensationAsync(() =>
        {
            actualCompensation();
            return Task.CompletedTask;
        }).GetAwaiter().GetResult(); // we know it will complete synchronously
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