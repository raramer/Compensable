using Compensable.Tests.Extensions;

namespace Compensable.Tests.ExceptionTests;

public class TagNotFoundExceptionTests
{
    [Fact]
    public void TagNotFound()
    {
        // act
        var tagNotFoundException = new TagNotFoundException().ThrowAndCatch();

        // assert
        Assert.NotNull(tagNotFoundException);
        Assert.Equal(ExpectedMessages.TagNotFound, tagNotFoundException.Message);
    }
}