namespace Compensable.Tests.ReadmeTests;

internal class Steps
{
    private Compensator? compensator = null;

    #pragma warning disable CS8602
    internal async Task AddCompensationAsync(bool compensate)
    {
        await compensator.AddCompensationAsync(
            compensation: async () => await compensateStepAsync(),
            compensateAtTag: null);

        Task compensateStepAsync() => Task.CompletedTask;
    }

    internal async Task CommitAsync()
    {
        await compensator.CommitAsync();
    }

    internal async Task CompensateAsync()
    {
        await compensator.CompensateAsync();
    }

    internal async Task CreateTagAsync()
    {
        var tag = await compensator.CreateTagAsync();
    }

    internal async Task DoAsync()
    {
        await compensator.DoAsync(
            execution: async () => await stepAsync(),
            compensation: async () => await compensateStepAsync(),
            compensateAtTag: null);

        Task compensateStepAsync() => Task.CompletedTask;
        Task stepAsync() => Task.CompletedTask;
    }

    internal async Task DoIfAsync()
    {
        await compensator.DoIfAsync(
            test: async () => await testAsync(),
            execution: async () => await stepAsync(),
            compensation: async () => await compensateStepAsync(),
            compensateAtTag: null);

        Task compensateStepAsync() => Task.CompletedTask;
        Task stepAsync() => Task.CompletedTask;
        Task<bool> testAsync() => Task.FromResult(true);
    }

    internal async Task ForeachAsync()
    {
        var items = new[] { "item1", "item2", "item3" };
        await compensator.ForeachAsync(
            items: items,
            execution: async (item) => await stepAsync(item),
            compensation: async (item) => await compensateStepAsync(item),
            compensateAtTag: null);

        Task compensateStepAsync(string item) => Task.CompletedTask;
        Task stepAsync(string item) => Task.CompletedTask;
    }

    internal async Task GetAsync()
    {
        var result = await compensator.GetAsync(
            execution: async () => await stepAsync(),
            compensation: async (_result) => await compensateStepAsync(_result),
            compensateAtTag: null);

        Task compensateStepAsync(int result) => Task.CompletedTask;
        Task<int> stepAsync() => Task.FromResult(1);
    }
    #pragma warning restore CS8602
}