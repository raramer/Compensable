namespace Compensable.Tests.CompensatorTests;

public class GetAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper(Compensation.ExpectToBeCalledAndThrowException);
        await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator, getHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper(Compensation.IsNull);

        // act
        var gotResult = await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper(Compensation.WouldThrowExceptionButNotCalled);

        // act
        var gotResult = await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator, getHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.ExpectToBeCalledAndThrowException);

        // act
        var exception = await Assert.ThrowsAsync<HelperExecutionException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.IsNull);

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper();

        // act
        var gotResult = await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator, getHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public async Task TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper(Execution.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = await Assert.ThrowsAsync<TagNotFoundException>(async () =>
            await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var getHelper = new GetHelper();
        var tag = default(Tag);

        // act
        var gotResult = await compensator.GetAsync(getHelper.ExecuteAsync, getHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        await AssertInternalCompensationOrderAsync(compensator, getHelper).ConfigureAwait(false);
    }
}