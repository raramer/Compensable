using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.AsyncCompensatorTests;

public class CompensateAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new AsyncCompensator();

        var arrangedTagsAndCompensations = await ArrangeTagsAndCompensationsAsync(compensator);
        var arrangedTag = arrangedTagsAndCompensations.OfType<Tag>().Skip(3).First();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync, arrangedTag);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator, arrangedTagsAndCompensations
            .SkipWhile(t => t != arrangedTag)
            .OfType<HelperBase>()
            .Reverse()
            .Append(addCompensationHelper)
            .Reverse()
            .ToArray());
    }

    [Fact]
    public async Task CompensationSucceeds()
    {
        // arrange
        var compensator = new AsyncCompensator();

        await ArrangeTagsAndCompensationsAsync(compensator);

        // act
        await compensator.CompensateAsync();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator);
    }

    [Fact]
    public async Task NothingToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        await compensator.CompensateAsync();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status);

        // act
        await compensator.CompensateAsync();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator);
    }

    [Fact]
    public async Task StatusIsCompensatingAndCompensationWillSucceed()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status);

        // act
        await compensator.CompensateAsync();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator);
    }

    [Fact]
    public async Task StatusIsCompensatingButCompensationWillFail()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CompensateAsync()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsFailedToExecuteAndCompensationWillSucceed()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status);

        // act
        await compensator.CompensateAsync();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator);
    }

    [Fact]
    public async Task StatusIsFailedToExecuteButCompensationWillFail()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        await compensator.AddCompensationAsync(addCompensationHelper.CompensateAsync);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);
    }
}