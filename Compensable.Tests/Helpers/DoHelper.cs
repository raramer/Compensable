﻿namespace Compensable.Tests.Helpers;

public class DoHelper : ExecuteCompensateHelperBase
{
    public DoHelper(string? label = null, bool throwOnExecute = false, bool throwOnCompensate = false, bool expectExecutionToBeCalled = true, bool expectCompensationToBeCalled = false) : base(
        label: label, 
        throwOnExecute: throwOnExecute, 
        throwOnCompensate: throwOnCompensate, 
        expectExecutionToBeCalled: expectExecutionToBeCalled,
        expectCompensationToBeCalled: expectCompensationToBeCalled)
    {
    }

    public async Task CompensateAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        CompensationCalled = true;
        CompensationCalledAt = DateTime.UtcNow;
        if (ThrowOnCompensate)
            throw new HelperCompensationException();
    }

    public async Task ExecuteAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        ExecutionCalled = true;
        if (ThrowOnExecute)
            throw new HelperExecutionException();
    }
}