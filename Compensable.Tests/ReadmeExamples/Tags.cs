namespace Compensable.Tests.ReadmeTests;

internal class Tags
{
    internal async Task Example()
    {
        var asyncCompensator = new AsyncCompensator();

        // create tag
        var tag = await asyncCompensator.CreateTagAsync();

        // step 1
        await asyncCompensator.DoAsync(
            async () => await step1Async(),
            compensation: async () => await compensateStep1Async());

        // step 2
        await asyncCompensator.DoAsync(
            async () => await step2Async(),
            compensation: async () => await compensateStep2Async(),
            compensateAtTag: tag);

        // compensate
        // compensateStep1Async will be called first, followed by compensateStep2Async.
        await asyncCompensator.CompensateAsync();
    }

    private Task compensateStep1Async() => Task.CompletedTask;

    private Task compensateStep2Async() => Task.CompletedTask;

    private Task step1Async() => Task.CompletedTask;

    private Task step2Async() => Task.CompletedTask;
}