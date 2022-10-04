namespace Compensable.Tests.Helpers;

public class ForeachHelper : HelperBase
{
    private readonly ItemHelper[] _itemHelpers;

    public IEnumerable<object> Items
    {
        get
        {
            foreach (var itemHelper in _itemHelpers)
                yield return itemHelper.Item;
        }
    }

    public ForeachHelper(params ItemHelper[] itemHelpers) : this(null, itemHelpers)
    {
    }
    public ForeachHelper(string label, params ItemHelper[] itemHelpers) : base(label)
    {
        _itemHelpers = itemHelpers;
    }

    public IEnumerable<HelperBase> ExpectedCompensationOrder => _itemHelpers
        .Where(i => !i.ExpectCompensationToBeCalled || i.ThrowOnCompensate)
        .Reverse();

    public override void AssertHelper()
    {
        foreach (var itemHelper in _itemHelpers)
            itemHelper.AssertHelper();
    }

    public async Task CompensateAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        var itemHelper = _itemHelpers.First(itemHelper => itemHelper.Item == item);
        await itemHelper.CompensateAsync(item).ConfigureAwait(false);
    }

    public async Task ExecuteAsync(object item)
    {
        await Task.Delay(1).ConfigureAwait(false);
        var itemHelper = _itemHelpers.First(itemHelper => itemHelper.Item == item);
        await itemHelper.ExecuteAsync(item).ConfigureAwait(false);
    }
}