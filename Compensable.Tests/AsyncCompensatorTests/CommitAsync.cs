namespace Compensable.Tests.AsyncCompensatorTests;

public class CommitAsync : TestBase
{
    [Fact]
    public async Task CompensationsAreDefined()
    {
        // arrange
        var compensator = new AsyncCompensator();
        await ArrangeTagsAndCompensationsAsync(compensator);

        // act
        await compensator.CommitAsync();

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator);
    }

    [Fact]
    public async Task NothingToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        await compensator.CommitAsync();

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

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
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CommitAsync()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CommitAsync()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
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
            await compensator.CommitAsync()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CommitAsync()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }
}