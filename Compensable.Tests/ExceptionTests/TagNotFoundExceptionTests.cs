using Compensable.Tests.Helpers.Extensions;

namespace Compensable.Tests.CompensationExceptionTests;

public class TagNotFoundExceptionTests
{
    [Fact]
    public void SerializedAndDeserialized()
    {
        // arrange
        var tagNotFoundException = new TagNotFoundException().ThrowAndCatch();

        // act
        var serializedAndDeserializedException = tagNotFoundException.SerializeAndDeserialize();

        // assert
        Assert.NotNull(serializedAndDeserializedException);
        Assert.Equal(tagNotFoundException.ToString(), serializedAndDeserializedException.ToString());
    }

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