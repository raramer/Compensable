namespace Compensable.Tests.CompensatorTests;

public class CreateTag : TestBase
{
    [Fact]
    public void LabelIsEmpty()
    {
        // arrange
        var compensator = new Compensator();

        // act
        var label = "";
        var tag = compensator.CreateTag(label);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void LabelIsNotSpecified()
    {
        // arrange
        var compensator = new Compensator();

        // act
        var tag = compensator.CreateTag();

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void LabelIsNull()
    {
        // arrange
        var compensator = new Compensator();

        // act
        var label = default(string);
        var tag = compensator.CreateTag(label);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void LabelIsSpecified()
    {
        // arrange
        var compensator = new Compensator();

        // act
        var label = "My label";
        var tag = compensator.CreateTag(label);

        // assert
        Assert.NotNull(tag);
        Assert.Equal(label, tag.Label);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        AssertInternalCompensationOrder(compensator);
    }

    [Fact]
    public void LabelIsWhitespace()
    {
        // arrange
        var compensator = new Compensator();

        // act
        var label = " ";
        var tag = compensator.CreateTag(label);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

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
            compensator.CreateTag()
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
            compensator.CreateTag()
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
            compensator.CreateTag()
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
            compensator.CreateTag()
        );

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }
}