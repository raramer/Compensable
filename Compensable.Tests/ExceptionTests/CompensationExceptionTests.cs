using Compensable.Tests.Extensions;

namespace Compensable.Tests.CompensationExceptionTests;

public class CompensationExceptionTests
{
    [Fact]
    public void WhileCompensatingIsNullAndWhileExecutingIsNull()
    {
        // arrange
        var whileCompensating = default(Exception);
        var whileExecuting = default(Exception);

        // act
        var compensationException = new CompensationException(whileCompensating, whileExecuting).ThrowAndCatch();

        // assert
        Assert.NotNull(compensationException);
        Assert.Equal(ExpectedMessages.WhileCompensating(string.Empty), compensationException.Message);
        Assert.Equal(whileCompensating, compensationException.InnerException);
        Assert.Equal(whileCompensating, compensationException.WhileCompensating);
        Assert.Null(compensationException.WhileExecuting);
    }

    [Fact]
    public void WhileCompensatingIsNullAndWhileExecutingIsNotNull()
    {
        // arrange
        var whileCompensating = default(Exception);
        var whileExecuting = new HelperExecutionException();

        // act
        var compensationException = new CompensationException(whileCompensating, whileExecuting).ThrowAndCatch();

        // assert
        Assert.NotNull(compensationException);
        Assert.Equal(ExpectedMessages.WhileExecutingWhileCompensating(whileCompensatingMessage: string.Empty), compensationException.Message);
        Assert.Equal(whileExecuting, compensationException.InnerException);
        Assert.Equal(whileCompensating, compensationException.WhileCompensating);
        Assert.Equal(whileExecuting, compensationException.WhileExecuting);
    }

    [Fact]
    public void WhileCompensatingIsNotNullAndWhileExecutingIsNull()
    {
        // arrange
        var whileCompensating = new HelperCompensationException();
        var whileExecuting = default(Exception);

        // act
        var compensationException = new CompensationException(whileCompensating, whileExecuting).ThrowAndCatch();

        // assert
        Assert.NotNull(compensationException);
        Assert.Equal(ExpectedMessages.WhileCompensating(), compensationException.Message);
        Assert.Equal(whileCompensating, compensationException.InnerException);
        Assert.Equal(whileCompensating, compensationException.WhileCompensating);
        Assert.Null(compensationException.WhileExecuting);
    }

    [Fact]
    public void WhileCompensatingIsNotNullAndWhileExecutingIsNotNull()
    {
        // arrange
        var whileCompensating = new HelperCompensationException();
        var whileExecuting = new HelperExecutionException();

        // act
        var compensationException = new CompensationException(whileCompensating, whileExecuting).ThrowAndCatch();

        // assert
        Assert.NotNull(compensationException);
        Assert.Equal(ExpectedMessages.WhileExecutingWhileCompensating(), compensationException.Message);
        Assert.Equal(whileExecuting, compensationException.InnerException);
        Assert.Equal(whileCompensating, compensationException.WhileCompensating);
        Assert.Equal(whileExecuting, compensationException.WhileExecuting);
    }
}