namespace Compensable.Tests.AsyncCompensatorTests;

public class ForeachAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndThrowException),
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndSucceed));
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator, foreachHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(ForeachOptions.CompensationNull).AddItems(
            new ItemHelper(),
            new ItemHelper(),
            new ItemHelper());

        // act
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillFail()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.WouldThrowExceptionButNotCalled),
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
    public async Task ExecutionFails()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndSucceed),
            new ItemHelper(ItemExecutionOptions.ShouldBeCalledAndThrowException),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

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
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(ForeachOptions.ExecutionNull).AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
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
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(),
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
    public async Task ItemEnumerationFails()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndSucceed),
            new ItemHelper(ItemEnumerationOptions.ShouldBeCalledAndThrowException),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

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
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper(ForeachOptions.ItemsNull).AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("items"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }


    [Fact]
    public async Task ItemsIsEmpty()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems();

        // act
        await compensator.ForeachAsync(foreachHelper.Items, foreachHelper.ExecuteAsync, foreachHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

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
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

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
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

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
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

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
        var compensator = new AsyncCompensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));
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
        var compensator = new AsyncCompensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(),
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
}