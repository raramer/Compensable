namespace Compensable.Tests.Helpers;

public class CompensateHelper : CompensateHelperBase
{
    public CompensateHelper(string? label = null, bool throwOnCompensate = false, bool expectCompensationToBeCalled = false) : base(
        label: label, 
        throwOnCompensate: throwOnCompensate, 
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
}