namespace Compensable.Tests.CompensatorTests;

public class DoIfAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper1 = new DoIfHelper(testResult: true, throwOnCompensate: true, expectCompensationToBeCalled: true);
        await compensator.DoIfAsync(doIfHelper1.TestAsync, doIfHelper1.ExecuteAsync, doIfHelper1.CompensateAsync, tag).ConfigureAwait(false);

        var doIfHelper2 = new DoIfHelper(testResult: true, throwOnExecute: true);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.DoIfAsync(doIfHelper2.TestAsync, doIfHelper2.ExecuteAsync, doIfHelper2.CompensateAsync, tag).ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(doIfHelper1, doIfHelper2);

        await AssertInternalCompensationOrderAsync(compensator, doIfHelper1).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        var doIfHelper = new DoIfHelper(true);
        Func<Task>? compensation = null;

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, compensation, tag).ConfigureAwait(false);

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

        var doIfHelper = new DoIfHelper(testResult: true, throwOnCompensate: true);

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

        var doIfHelper = new DoIfHelper(testResult: true, throwOnExecute: true);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);
        Func<Task>? execution = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.DoIfAsync(doIfHelper.TestAsync, execution, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
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

        var doIfHelper = new DoIfHelper(testResult: true);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);
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

        var doIfHelper = new DoIfHelper(testResult: true);
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

        var doIfHelper = new DoIfHelper(testResult: true, throwOnTest: true);

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

        var doIfHelper = new DoIfHelper(testResult: true, expectTestToBeCalled: false);
        Func<Task<bool>>? test = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await compensator.DoIfAsync(test, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false)
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

        var doIfHelper = new DoIfHelper(testResult: false);

        // act
        await compensator.DoIfAsync(doIfHelper.TestAsync, doIfHelper.ExecuteAsync, doIfHelper.CompensateAsync, tag).ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }
}