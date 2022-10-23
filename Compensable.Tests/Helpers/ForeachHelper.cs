using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class ForeachHelper : HelperBase
{
    private readonly List<ItemHelper> _itemHelpers = new List<ItemHelper>();

    public Func<object, Task>? CompensateAsync { get; }

    public Func<object, Task>? ExecuteAsync { get; }

    public IEnumerable<object>? Items { get; }

    private Foreach.Options Options { get; }

    public ForeachHelper(string? label = null)
        : this(Foreach.NothingIsNull, label)
    {
    }

    public ForeachHelper(Foreach.Options options, string? label = null) : base(label)
    {
        Options = options;

        Items = Options.ItemsIsNull ? null : _Enumerate();
        ExecuteAsync = Options.ExecutionIsNull ? null : _ExecuteAsync;
        CompensateAsync = Options.CompensationIsNull ? null : _CompensateAsync;
    }

    public ForeachHelper AddItems(params ItemHelper[] itemHelper)
    {
        _itemHelpers.AddRange(itemHelper);
        return this;
    }

    public override void AssertHelper()
    {
        foreach (var itemHelper in _itemHelpers)
            itemHelper.AssertHelper();
    }

    public IEnumerable<HelperBase> GetExpectedCompensationOrder()
    {
        return _itemHelpers.Where(_ => _.ExpectInCompensationStack).Reverse();
    }

    public override Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation)
    {
        throw new NotImplementedException();
    }

    private async Task _CompensateAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        var itemHelper = _itemHelpers.First(itemHelper => itemHelper.ExpectedItem == item);
        await itemHelper.CompensateAsync(item).ConfigureAwait(false);
    }

    private IEnumerable<object> _Enumerate()
    {
        foreach (var itemHelper in _itemHelpers)
            yield return itemHelper.Item;
    }

    private async Task _ExecuteAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        var itemHelper = _itemHelpers.First(itemHelper => itemHelper.ExpectedItem == item);
        await itemHelper.ExecuteAsync(item).ConfigureAwait(false);
    }
}