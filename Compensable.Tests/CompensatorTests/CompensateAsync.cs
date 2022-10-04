﻿namespace Compensable.Tests.CompensatorTests;

public class CompensateAsync : TestBase
{
    [Fact]
    public async Task CompensationFails()
    {
        // arrange
        var compensator = new Compensator();

        var arrangedTagsAndCompensations = await ArrangeTagsAndCompensationsAsync(compensator).ConfigureAwait(false);
        var arrangedTag = arrangedTagsAndCompensations.OfType<Tag>().Skip(3).First();

        var compensateHelper = new CompensateHelper(throwOnCompensate: true, expectCompensationToBeCalled: true);
        await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, arrangedTag).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator, arrangedTagsAndCompensations
            .SkipWhile(t => t != arrangedTag)
            .OfType<HelperBase>()
            .Reverse()
            .Append(compensateHelper)
            .Reverse()
            .ToArray()).ConfigureAwait(false);
    }

    [Fact]
    public async Task CompensationSucceeds()
    {
        // arrange
        var compensator = new Compensator();

        await ArrangeTagsAndCompensationsAsync(compensator).ConfigureAwait(false);

        // act
        await compensator.CompensateAsync().ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task NothingToCompensate()
    {
        // arrange
        var compensator = new Compensator();

        // act
        await compensator.CompensateAsync().ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        await compensator.CompensateAsync().ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensatingAndCompensationWillSucceed()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        await compensator.CompensateAsync().ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensatingButCompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var compensateHelper = new CompensateHelper(throwOnCompensate: true);
        await compensator.AddCompensationAsync(compensateHelper.CompensateAsync).ConfigureAwait(false);
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsFailedToExecuteAndCompensationWillSucceed()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        await compensator.CompensateAsync().ConfigureAwait(false);

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsFailedToExecuteButCompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var compensateHelper = new CompensateHelper(throwOnCompensate: true);
        await compensator.AddCompensationAsync(compensateHelper.CompensateAsync).ConfigureAwait(false);
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
            await compensator.CompensateAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        AssertCompensationException(exception, expectExecutionException: false);
    }
}