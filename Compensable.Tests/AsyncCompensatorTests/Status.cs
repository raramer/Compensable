namespace Compensable.Tests.AsyncCompensatorTests;

public class Status : TestBase
{
    [Fact]
    public async Task CompensatorFailedToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // assert
        Assert.Equal(status, compensator.Status);
    }

    [Fact]
    public async Task CompensatorIsCompensating()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // assert
        Assert.Equal(status, compensator.Status);
    }

    [Fact]
    public void CompensatorIsExecuting()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);
    }

    [Fact]
    public async Task CompensatorIsFailedToExecute()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // assert
        Assert.Equal(status, compensator.Status);
    }

    [Fact]
    public async Task CompensatorWasCompensated()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // assert
        Assert.Equal(status, compensator.Status);
    }
}