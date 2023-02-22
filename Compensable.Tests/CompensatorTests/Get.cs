namespace Compensable.Tests.CompensatorTests;

public class Get : TestBase
{
    [Fact]
    public void CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var getHelper = new GetHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.Get(getHelper.Execute, getHelper.Compensate, tag);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator, getHelper);
    }

    [Fact]
    public void CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var getHelper = new GetHelper(CompensationOptions.Null);

        // act
        var gotResult = compensator.Get(getHelper.Execute, getHelper.Compensate, tag);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var getHelper = new GetHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        var gotResult = compensator.Get(getHelper.Execute, getHelper.Compensate, tag);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator, getHelper);
    }

    [Fact]
    public void ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var getHelper = new GetHelper(ExecutionOptions.ExpectToBeCalledAndThrowException);

        // act
        var exception = Assert.Throws<HelperExecutionException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var getHelper = new GetHelper(ExecutionOptions.Null);

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var getHelper = new GetHelper();

        // act
        var gotResult = compensator.Get(getHelper.Execute, getHelper.Compensate, tag);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator, getHelper);
    }

    [Fact]
    public void StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        var getHelper = new GetHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public void StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        var getHelper = new GetHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public void StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        var getHelper = new GetHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public void StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        var getHelper = new GetHelper(ExecutionOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(getHelper);
    }

    [Fact]
    public void TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var getHelper = new GetHelper(ExecutionOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = Assert.Throws<TagNotFoundException>(() =>
            compensator.Get(getHelper.Execute, getHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var getHelper = new GetHelper();
        var tag = default(Tag);

        // act
        var gotResult = compensator.Get(getHelper.Execute, getHelper.Compensate, tag);

        // assert
        Assert.Equal(getHelper.ExpectedExecuteResult, gotResult);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(getHelper);

        AssertInternalCompensationOrder(compensator, getHelper);
    }
}