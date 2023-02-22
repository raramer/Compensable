namespace Compensable.Tests.CompensatorTests;

public class Do : TestBase
{
    [Fact]
    public void CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doHelper = new DoHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.Do(doHelper.Execute, doHelper.Compensate, tag);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator, doHelper);
    }

    [Fact]
    public void CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doHelper = new DoHelper(CompensationOptions.Null);

        // act
        compensator.Do(doHelper.Execute, doHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doHelper = new DoHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        compensator.Do(doHelper.Execute, doHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator, doHelper);
    }

    [Fact]
    public void ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doHelper = new DoHelper(ExecutionOptions.ExpectToBeCalledAndThrowException);

        // act
        var exception = Assert.Throws<HelperExecutionException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doHelper = new DoHelper(ExecutionOptions.Null);

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doHelper = new DoHelper();

        // act
        compensator.Do(doHelper.Execute, doHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator, doHelper);
    }

    [Fact]
    public void StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public void StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public void StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public void StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doHelper);
    }

    [Fact]
    public void TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var doHelper = new DoHelper(ExecutionOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = Assert.Throws<TagNotFoundException>(() =>
            compensator.Do(doHelper.Execute, doHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var doHelper = new DoHelper();
        var tag = default(Tag);

        // act
        compensator.Do(doHelper.Execute, doHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doHelper);

        AssertInternalCompensationOrder(compensator, doHelper);
    }
}