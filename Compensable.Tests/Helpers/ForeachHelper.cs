using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.Helpers;

public class ForeachHelper : HelperBase
{
    private readonly List<ItemHelper> _itemHelpers = new List<ItemHelper>();

    public Action<object>? Compensate { get; }

    public Func<object, Task>? CompensateAsync { get; }

    public Action<object>? Execute { get; }

    public Func<object, Task>? ExecuteAsync { get; }

    public IEnumerable<object>? Items { get; }

    private ForeachOptions ForeachOptions { get; }

    public ForeachHelper(string? label = null)
        : this(ForeachOptions.NothingNull, label)
    {
    }

    public ForeachHelper(ForeachOptions foreachOptions, string? label = null) : base(label)
    {
        ForeachOptions = foreachOptions;

        Items = ForeachOptions.ItemsIsNull ? null : _Enumerate();

        if (!ForeachOptions.ExecutionIsNull)
        {
            Execute = _Execute;
            ExecuteAsync = _ExecuteAsync;
        }

        if (!ForeachOptions.CompensationIsNull)
        {
            Compensate = _Compensate;
            CompensateAsync = _CompensateAsync;
        }
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

    public override bool IsExpectedCompensation(Action actualCompensation)
    {
        throw new NotImplementedException();
    }


    public override Task<bool> IsExpectedCompensationAsync(Func<Task> actualCompensation)
    {
        throw new NotImplementedException();
    }

    private void _Compensate(object item)
    {
        var itemHelper = _itemHelpers.First(itemHelper => itemHelper.ExpectedItem == item);
        itemHelper.Compensate(item);
    }
    private async Task _CompensateAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        _Compensate(item);
    }

    private IEnumerable<object> _Enumerate()
    {
        foreach (var itemHelper in _itemHelpers)
            yield return itemHelper.Item;
    }

    private void _Execute(object item)
    {
        var itemHelper = _itemHelpers.First(itemHelper => itemHelper.ExpectedItem == item);
        itemHelper.Execute(item);
    }

    private async Task _ExecuteAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        _Execute(item);
    }
}