namespace Compensable.Tests.CompensatorTests;

public class DoIf : TestBase
{
    [Fact]
    public void CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(CompensationOptions.ExpectToBeCalledAndThrowException);
        compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator, doIfHelper);
    }

    [Fact]
    public void CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(CompensationOptions.Null);

        // act
        compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(CompensationOptions.WouldThrowExceptionButNotCalled);

        // act
        compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator, doIfHelper);
    }

    [Fact]
    public void ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(ExecutionOptions.ExpectToBeCalledAndThrowException);

        // act
        var exception = Assert.Throws<HelperExecutionException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(ExecutionOptions.Null);

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper();

        // act
        compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator, doIfHelper);
    }

    [Fact]
    public void StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public void StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public void StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public void StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(doIfHelper);
    }

    [Fact]
    public void TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(TestOptions.WouldSucceedButNotCalled);
        var tag = new Tag();

        // act
        var exception = Assert.Throws<TagNotFoundException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper();
        var tag = default(Tag);

        // act
        compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator, doIfHelper);
    }

    [Fact]
    public void TestFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(TestOptions.ShouldBeCalledAndThrowException);

        // act
        var exception = Assert.Throws<HelperTestException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.TestFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TestIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(TestOptions.Null);

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("test"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TestResultIsFalse()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var doIfHelper = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnFalse);

        // act
        compensator.DoIf(doIfHelper.Test, doIfHelper.Execute, doIfHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(doIfHelper);

        AssertInternalCompensationOrder(compensator);
    }
}