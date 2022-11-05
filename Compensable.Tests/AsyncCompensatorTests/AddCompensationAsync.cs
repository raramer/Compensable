namespace Compensable.Tests.AsyncCompensatorTests;

public class AddCompensationAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(addCompensationHelper);

        await AssertInternalCompensationOrderAsync(compensator, addCompensationHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.Null);

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("compensation"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(addCompensationHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillFail()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(addCompensationHelper);

        await AssertInternalCompensationOrderAsync(compensator, addCompensationHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillSucceed()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(addCompensationHelper);

        await AssertInternalCompensationOrderAsync(compensator, addCompensationHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public async Task TagDoesNotExist()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = await Assert.ThrowsAsync<TagNotFoundException>(async () =>
            await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(addCompensationHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TagIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);
        var tag = default(Tag);

        // act
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(addCompensationHelper);

        await AssertInternalCompensationOrderAsync(compensator, addCompensationHelper).ConfigureAwait(false);
    }
}