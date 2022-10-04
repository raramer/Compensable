using Compensable.Tests.Helpers.Extensions;

namespace Compensable.Tests.CompensationExceptionTests;

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