namespace Compensable.Tests.AsyncCompensatorTests;

public class CreateTagAsync : TestBase
{
    [Fact]
    public async Task LabelIsEmpty()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        var label = "";
        var tag = await compensator.CreateTagAsync(label).ConfigureAwait(false);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task LabelIsNotSpecified()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task LabelIsNull()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        var label = default(string);
        var tag = await compensator.CreateTagAsync(label).ConfigureAwait(false);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task LabelIsSpecified()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        var label = "My label";
        var tag = await compensator.CreateTagAsync(label).ConfigureAwait(false);

        // assert
        Assert.NotNull(tag);
        Assert.Equal(label, tag.Label);

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task LabelIsWhitespace()
    {
        // arrange
        var compensator = new AsyncCompensator();

        // act
        var label = " ";
        var tag = await compensator.CreateTagAsync(label).ConfigureAwait(false);

        // assert
        Assert.NotNull(tag);
        Assert.True(Guid.TryParse(tag.Label, out _));

        Assert.Equal(CompensatorStatus.Executing, compensator.Status);

        await AssertInternalCompensationOrderAsync(compensator).ConfigureAwait(false);
    }

    [Fact]
    public async Task StatusIsCompensated()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensated;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CreateTagAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsCompensating()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.Compensating;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CreateTagAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsFailedToCompensate()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToCompensate;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CreateTagAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }

    [Fact]
    public async Task StatusIsFailedToExecute()
    {
        // arrange
        var compensator = new AsyncCompensator();
        var status = CompensatorStatus.FailedToExecute;
        await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

        // act
        var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
            await compensator.CreateTagAsync().ConfigureAwait(false)
        ).ConfigureAwait(false);

        // assert
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
    }
}