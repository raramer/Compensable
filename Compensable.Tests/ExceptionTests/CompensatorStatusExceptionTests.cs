using Compensable.Tests.Extensions;

namespace Compensable.Tests.ExceptionTests;

public class CompensatorStatusExceptionTests
{
    [Fact]
    public void CompensatorStatusIsCompensated()
    {
        // arrange
        var compensatorStatus = CompensatorStatus.Compensated;

        // act
        var compensatorStatusException = new CompensatorStatusException(compensatorStatus).ThrowAndCatch();

        // assert
        Assert.NotNull(compensatorStatusException);
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(compensatorStatus), compensatorStatusException.Message);
    }

    [Fact]
    public void CompensatorStatusIsCompensating()
    {
        // arrange
        var compensatorStatus = CompensatorStatus.Compensating;

        // act
        var compensatorStatusException = new CompensatorStatusException(compensatorStatus).ThrowAndCatch();

        // assert
        Assert.NotNull(compensatorStatusException);
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(compensatorStatus), compensatorStatusException.Message);
    }

    [Fact]
    public void CompensatorStatusIsExecuting()
    {
        // arrange
        var compensatorStatus = CompensatorStatus.Executing;

        // act
        var compensatorStatusException = new CompensatorStatusException(compensatorStatus).ThrowAndCatch();

        // assert
        Assert.NotNull(compensatorStatusException);
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(compensatorStatus), compensatorStatusException.Message);
    }

    [Fact]
    public void CompensatorStatusIsFailedToCompensate()
    {
        // arrange
        var compensatorStatus = CompensatorStatus.FailedToCompensate;

        // act
        var compensatorStatusException = new CompensatorStatusException(compensatorStatus).ThrowAndCatch();

        // assert
        Assert.NotNull(compensatorStatusException);
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(compensatorStatus), compensatorStatusException.Message);
    }

    [Fact]
    public void CompensatorStatusIsFailedToExecute()
    {
        // arrange
        var compensatorStatus = CompensatorStatus.FailedToExecute;

        // act
        var compensatorStatusException = new CompensatorStatusException(compensatorStatus).ThrowAndCatch();

        // assert
        Assert.NotNull(compensatorStatusException);
        Assert.Equal(ExpectedMessages.CompensatorStatusIs(compensatorStatus), compensatorStatusException.Message);
    }
}