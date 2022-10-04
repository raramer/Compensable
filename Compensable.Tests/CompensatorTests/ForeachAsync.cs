namespace Compensable.Tests.CompensatorTests;

public class ForeachAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var foreachHelper1 = new ForeachHelper(
            new ItemHelper(throwOnCompensate: true, expectCompensationToBeCalled: true),
            new ItemHelper(expectCompensationToBeCalled: true));
        await compensator.ForeachAsync(foreachHelper1.Items, foreachHelper1.ExecuteAsync, foreachHelper1.CompensateAsync, tag).ConfigureAwait(false);

        var foreachHelper2 = new ForeachHelper(
            new ItemHelper(expectCompensationToBeCalled: true),
            new ItemHelper(throwOnExecute: true));

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.ForeachAsync(foreachHelper2.Items, foreachHelper2.ExecuteAsync, foreachHelper2.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(foreachHelper1, foreachHelper2);

        await AssertInternalCompensationOrderAsync(compensator, foreachHelper1).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(),
            new ItemHelper());
        Func<object, Task>? compensation = null;

        // act
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, compensation, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(throwOnCompensate: true),
            new ItemHelper());

        // act
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator, foreachHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectCompensationToBeCalled: true),
            new ItemHelper(throwOnExecute: true));

        // act
        var exception = await Assert.ThrowsAsync<HelperExecutionException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));
        Func<object, Task>? execution = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, execution, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(),
            new ItemHelper());

        // act
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator, foreachHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public async Task TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));
        var tag = new Tag();

        // act
        var exception = await Assert.ThrowsAsync<TagNotFoundException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(),
            new ItemHelper());
        var tag = default(Tag);

        // act
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator, foreachHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task ItemEnumerationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectCompensationToBeCalled: true),
            new ItemHelper(throwOnItem: true));

        // act
        var exception = await Assert.ThrowsAsync<HelperItemException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ItemFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ItemsIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(
            new ItemHelper(expectItemToBeCalled: false),
            new ItemHelper(expectItemToBeCalled: false));
        IEnumerable<object>? items = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.ForeachAsync(items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("items"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }
}