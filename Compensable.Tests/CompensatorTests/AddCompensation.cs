namespace Compensable.Tests.CompensatorTests;

public class AddCompensation : TestBase
{
    [Fact]
    public void CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.AddCompensation(addCompensationHelper.Compensate, tag);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(addCompensationHelper);

        AssertInternalCompensationOrder(compensator, addCompensationHelper);
    }

    [Fact]
    public void CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.Null);

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.AddCompensation(addCompensationHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("compensation"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(addCompensationHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        compensator.AddCompensation(addCompensationHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(addCompensationHelper);

        AssertInternalCompensationOrder(compensator, addCompensationHelper);
    }

    [Fact]
    public void CompensationWillSucceed()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        compensator.AddCompensation(addCompensationHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(addCompensationHelper);

        AssertInternalCompensationOrder(compensator, addCompensationHelper);
    }

    [Fact]
    public void StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.AddCompensation(addCompensationHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public void StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.AddCompensation(addCompensationHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public void StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.AddCompensation(addCompensationHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public void StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.AddCompensation(addCompensationHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(addCompensationHelper);
    }

    [Fact]
    public void TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = Assert.Throws<TagNotFoundException>(() =>
            compensator.AddCompensation(addCompensationHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(addCompensationHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var addCompensationHelper = new AddCompensationHelper(CompensationOptions.WouldSucceedButNotCalled);
        var tag = default(Tag);

        // act
        compensator.AddCompensation(addCompensationHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(addCompensationHelper);

        AssertInternalCompensationOrder(compensator, addCompensationHelper);
    }
}