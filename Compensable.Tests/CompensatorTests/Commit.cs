namespace Compensable.Tests.CompensatorTests;

public class Commit : TestBase
{
    [Fact]
    public void CompensationsAreDefined()
    {
        // arrange
        var compensator = new Compensator();
        ArrangeTagsAndCompensations(compensator);

        // act
        compensator.Commit();

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void NothingToCompensate()
    {
        // arrange
        var compensator = new Compensator();

        // act
        compensator.Commit();

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Commit()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public void StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Commit()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public void StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Commit()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public void StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Commit()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }
}