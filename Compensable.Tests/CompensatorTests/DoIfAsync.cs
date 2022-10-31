namespace Compensable.Tests.CompensatorTests;

public class DoIfAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator, doIfHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(CompensationOptions.Null);

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator, doIfHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(ExecutionOptions.ExpectToBeCalledAndThrowException);

        // act
        var exception = await Assert.ThrowsAsync<HelperExecutionException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(ExecutionOptions.Null);

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper();

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator, doIfHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public async Task TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = await Assert.ThrowsAsync<TagNotFoundException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper();
        var tag = default(Tag);

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator, doIfHelper).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.ShouldBeCalledAndThrowException);

        // act
        var exception = await Assert.ThrowsAsync<HelperTestException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.TestFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.Null);

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("test"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestResultIsFalse()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnFalse);

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }
}