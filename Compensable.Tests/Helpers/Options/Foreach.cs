namespace Compensable.Tests.Helpers.Options;

public static class Foreach
{
    public sealed record Options(
        bool ItemsIsNull = false, 
        bool ExecutionIsNull = false, 
        bool CompensationIsNull = false);

    public static Options CompensationIsNull
        => new Options(ItemsIsNull: false, ExecutionIsNull: false, CompensationIsNull: true);

    public static Options ItemsIsNull
        => new Options(ItemsIsNull: true, ExecutionIsNull: false, CompensationIsNull: false);

    public static Options ExecutionIsNull
        => new Options(ItemsIsNull: false, ExecutionIsNull: true, CompensationIsNull: false);

    public static Options NothingIsNull
        => new Options(ItemsIsNull: false, ExecutionIsNull: false, CompensationIsNull: false);
}
