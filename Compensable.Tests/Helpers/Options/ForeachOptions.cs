namespace Compensable.Tests.Helpers.Options;

public sealed record ForeachOptions(
    bool ItemsIsNull = false, 
    bool ExecutionIsNull = false, 
    bool CompensationIsNull = false)
{
    public static ForeachOptions CompensationNull
        => new ForeachOptions(ItemsIsNull: false, ExecutionIsNull: false, CompensationIsNull: true);

    public static ForeachOptions ItemsNull
        => new ForeachOptions(ItemsIsNull: true, ExecutionIsNull: false, CompensationIsNull: false);

    public static ForeachOptions ExecutionNull
        => new ForeachOptions(ItemsIsNull: false, ExecutionIsNull: true, CompensationIsNull: false);

    public static ForeachOptions NothingNull
        => new ForeachOptions(ItemsIsNull: false, ExecutionIsNull: false, CompensationIsNull: false);
}
