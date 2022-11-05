namespace Compensable.Tests.AsyncCompensatorTests;

public class DoAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator, doHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper(CompensationOptions.Null);

        // act
        await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillFail()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator, doHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionFails()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.ExpectToBeCalledAndThrowException);

        // act
        var exception = await Assert.ThrowsAsync<HelperExecutionException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.Null);

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionSucceeds()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper();

        // act
        await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator, doHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public async Task TagDoesNotExist()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = await Assert.ThrowsAsync<TagNotFoundException>(async () =>
            await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TagIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doHelper = new DoHelper();
        var tag = default(Tag);

        // act
        await compensator.DoAsync(doHelper.ExecuteAsync, doHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        await AssertInternalCompensationOrderAsync(compensator, doHelper).ConfigureAwait(false);
    }
}