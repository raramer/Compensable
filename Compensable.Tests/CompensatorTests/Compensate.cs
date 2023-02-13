using Compensable.Tests.Helpers.Bases;

namespace Compensable.Tests.CompensatorTests;

public class Compensate : TestBase
{
    [Fact]
    public void CompensationFails()
    {
        // arrange
        var compensator = new Compensator();

        var arrangedTagsAndCompensations = ArrangeTagsAndCompensations(compensator);
        var arrangedTag = arrangedTagsAndCompensations.OfType<Tag>().Skip(3).First();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.AddCompensation(addCompensationHelper.Compensate, arrangedTag);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertInternalCompensationOrder(compensator, arrangedTagsAndCompensations
            .SkipWhile(t => t != arrangedTag)
            .OfType<HelperBase>()
            .Reverse()
            .Append(addCompensationHelper)
            .Reverse()
            .ToArray());
    }

    [Fact]
    public void CompensationSucceeds()
    {
        // arrange
        var compensator = new Compensator();

        ArrangeTagsAndCompensations(compensator);

        // act
        compensator.Compensate();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void NothingToCompensate()
    {
        // arrange
        var compensator = new Compensator();

        // act
        compensator.Compensate();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

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
        compensator.Compensate();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void StatusIsCompensatingAndCompensationWillSucceed()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        // act
        compensator.Compensate();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void StatusIsCompensatingButCompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.AddCompensation(addCompensationHelper.Compensate);
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);
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
            compensator.Compensate()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public void StatusIsFailedToExecuteAndCompensationWillSucceed()
    {
        // arrange
        var compensator = new Compensator();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        // act
        compensator.Compensate();

        // assert
        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void StatusIsFailedToExecuteButCompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.AddCompensation(addCompensationHelper.Compensate);
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);
    }
}