namespace Compensable.Tests.CompensatorTests;

public class Status : TestBase
{
    [Fact]
    public void CompensatorFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        // assert
        Assert.Equal(status, compensator.Status);
    }

    [Fact]
    public void CompensatorIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        // assert
        Assert.Equal(status, compensator.Status);
    }

    [Fact]
    public void CompensatorIsExecuting()
    {
        // arrange
        var compensator = new Compensator();

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);
    }

    [Fact]
    public void CompensatorIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        // assert
        Assert.Equal(status, compensator.Status);
    }

    [Fact]
    public void CompensatorWasCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        // assert
        Assert.Equal(status, compensator.Status);
    }
}