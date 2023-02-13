namespace Compensable.Tests.CompensatorTests;

public class Foreach : TestBase
{
    [Fact]
    public void CompensationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndThrowException),
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndSucceed));
        compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag);

        // act
        var exception = Assert.Throws<CompensationException>(() =>
            compensator.Compensate()
        );

        // assert
        AssertCompensationException(exception, expectExecutionException: false);

        Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator, foreachHelper);
    }

    [Fact]
    public void CompensationIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper(ForeachOptions.CompensationNull).AddItems(
            new ItemHelper(),
            new ItemHelper(),
            new ItemHelper());

        // act
        compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void CompensationWillFail()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.WouldThrowExceptionButNotCalled),
            new ItemHelper(),
            new ItemHelper());

        // act
        compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator, foreachHelper);
    }

    [Fact]
    public void ExecutionFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndSucceed),
            new ItemHelper(ItemExecutionOptions.ShouldBeCalledAndThrowException),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<HelperExecutionException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ExecuteFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper(ForeachOptions.ExecutionNull).AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("execution"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ExecutionSucceeds()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(),
            new ItemHelper(),
            new ItemHelper());

        // act
        compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator, foreachHelper);
    }

    [Fact]
    public void ItemEnumerationFails()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemCompensationOptions.ShouldBeCalledAndSucceed),
            new ItemHelper(ItemEnumerationOptions.ShouldBeCalledAndThrowException),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<HelperItemException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ItemFailed, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void ItemsIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper(ForeachOptions.ItemsNull).AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.ValueCannotBeNull("items"), exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }


    [Fact]
    public void ItemsIsEmpty()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems();

        // act
        compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void StatusIsCompensated()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensated;
        ArrangeStatus(compensator, status);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public void StatusIsCompensating()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.Compensating;
        ArrangeStatus(compensator, status);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public void StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToCompensate;
        ArrangeStatus(compensator, status);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public void StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new Compensator();
        var tag = compensator.CreateTag();
        var status = CompensatorStatus.FailedToExecute;
        ArrangeStatus(compensator, status);

        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));

        // act
        var exception = Assert.Throws<CompensatorStatusException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

        AssertHelpers(foreachHelper);
    }

    [Fact]
    public void TagDoesNotExist()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled),
            new ItemHelper(ItemEnumerationOptions.WouldSucceedButNotCalled));
        var tag = new Tag();

        // act
        var exception = Assert.Throws<TagNotFoundException>(() =>
            compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag)
        );

        // assert
        Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

        Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void TagIsNull()
    {
        // arrange
        var compensator = new Compensator();
        var unusedTag = compensator.CreateTag();
        var foreachHelper = new ForeachHelper().AddItems(
            new ItemHelper(),
            new ItemHelper(),
            new ItemHelper());
        var tag = default(Tag);

        // act
        compensator.Foreach(foreachHelper.Items, foreachHelper.Execute, foreachHelper.Compensate, tag);

        // assert
        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertHelpers(foreachHelper);

        AssertInternalCompensationOrder(compensator, foreachHelper);
    }
}