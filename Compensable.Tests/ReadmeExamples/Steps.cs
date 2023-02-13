namespace Compensable.Tests.ReadmeTests;

internal class Steps
{
    private Compensator? compensator = null;
    private AsyncCompensator? asyncCompensator = null;

    #pragma warning disable CS8602
    internal void AddCompensation(bool compensate)
    {
        compensator.AddCompensation(
            compensation: () => compensateStep(),
            compensateAtTag: null);

        void compensateStep() { };
    }

    internal async Task AddCompensationAsync(bool compensate)
    {
        await asyncCompensator.AddCompensationAsync(
            compensation: async () => await compensateStepAsync(),
            compensateAtTag: null);

        Task compensateStepAsync() => Task.CompletedTask;
    }

    internal void Commit()
    {
        compensator.Commit();
    }

    internal async Task CommitAsync()
    {
        await asyncCompensator.CommitAsync();
    }

    internal void Compensate()
    {
        compensator.Compensate();
    }

    internal async Task CompensateAsync()
    {
        await asyncCompensator.CompensateAsync();
    }

    internal void CreateTag()
    {
        var tag = compensator.CreateTag();
    }

    internal async Task CreateTagAsync()
    {
        var tag = await asyncCompensator.CreateTagAsync();
    }

    internal void Do()
    {
        compensator.Do(
            execution: () => step(),
            compensation: () => compensateStep(),
            compensateAtTag: null);

        void compensateStep() { };
        void step() { };
    }

    internal async Task DoAsync()
    {
        await asyncCompensator.DoAsync(
            execution: async () => await stepAsync(),
            compensation: async () => await compensateStepAsync(),
            compensateAtTag: null);

        Task compensateStepAsync() => Task.CompletedTask;
        Task stepAsync() => Task.CompletedTask;
    }

    internal void DoIf()
    {
        compensator.DoIf(
            test: () => test(),
            execution: () => step(),
            compensation: () => compensateStep(),
            compensateAtTag: null);

        void compensateStep() { };
        void step() { };
        bool test() => true;
    }

    internal async Task DoIfAsync()
    {
        await asyncCompensator.DoIfAsync(
            test: async () => await testAsync(),
            execution: async () => await stepAsync(),
            compensation: async () => await compensateStepAsync(),
            compensateAtTag: null);

        Task compensateStepAsync() => Task.CompletedTask;
        Task stepAsync() => Task.CompletedTask;
        Task<bool> testAsync() => Task.FromResult(true);
    }

    internal void Foreach()
    {
        var items = new[] { "item1", "item2", "item3" };
        compensator.Foreach(
            items: items,
            execution: (item) => step(item),
            compensation: (item) => compensateStep(item),
            compensateAtTag: null);

        void compensateStep(string item) { };
        void step(string item) { };
    }

    internal async Task ForeachAsync()
    {
        var items = new[] { "item1", "item2", "item3" };
        await asyncCompensator.ForeachAsync(
            items: items,
            execution: async (item) => await stepAsync(item),
            compensation: async (item) => await compensateStepAsync(item),
            compensateAtTag: null);

        Task compensateStepAsync(string item) => Task.CompletedTask;
        Task stepAsync(string item) => Task.CompletedTask;
    }

    internal void Get()
    {
        var result = compensator.Get(
            execution: () => step(),
            compensation: (_result) => compensateStep(_result),
            compensateAtTag: null);

        void compensateStep(int result) { };
        int step() => 1;
    }

    internal async Task GetAsync()
    {
        var result = await asyncCompensator.GetAsync(
            execution: async () => await stepAsync(),
            compensation: async (_result) => await compensateStepAsync(_result),
            compensateAtTag: null);

        Task compensateStepAsync(int result) => Task.CompletedTask;
        Task<int> stepAsync() => Task.FromResult(1);
    }
    #pragma warning restore CS8602
}