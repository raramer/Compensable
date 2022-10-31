using Compensable.Tests.Helpers.Bases;
using System.Collections.Concurrent;
using System.Reflection;

namespace Compensable.Tests.CompensatorTests;

public abstract class TestBase
{
    protected async Task ArrangeStatusAsync(Compensator compensator, CompensatorStatus status)
    {
        switch (status)
        {
            case CompensatorStatus.FailedToExecute:
            case CompensatorStatus.Compensating:
                typeof(Compensator)
                    .GetProperty(nameof(Compensator.Status), BindingFlags.Public | BindingFlags.Instance)?
                    .SetValue(compensator, status);
                break;

            case CompensatorStatus.Compensated:
                await compensator.CompensateAsync().ConfigureAwait(false);
                break;

            case CompensatorStatus.FailedToCompensate:
                await compensator.AddCompensationAsync(() => throw new InvalidOperationException("Failed to compensate")).ConfigureAwait(false);
                try
                {
                    await compensator.CompensateAsync().ConfigureAwait(false);
                }
                catch
                {
                    // Ignore exceptions
                }
                break;
        }
    }

    protected async Task<object[]> ArrangeTagsAndCompensationsAsync(Compensator compensator)
    {
        var foreachTag = await compensator.CreateTagAsync("foreachTag").ConfigureAwait(false);

        var doTag = await compensator.CreateTagAsync("doTag").ConfigureAwait(false);

        var doHelper1 = new DoHelper("doHelper1");
        await compensator.DoAsync(doHelper1.ExecuteAsync, doHelper1.CompensateAsync, doTag).ConfigureAwait(false);

        var doHelper2 = new DoHelper("doHelper2");
        await compensator.DoAsync(doHelper2.ExecuteAsync, doHelper2.CompensateAsync, doTag).ConfigureAwait(false);

        var doHelper3 = new DoHelper("doHelper3");
        await compensator.DoAsync(doHelper3.ExecuteAsync, doHelper3.CompensateAsync).ConfigureAwait(false);

        var doIfTag = await compensator.CreateTagAsync("doIfTag").ConfigureAwait(false);

        var doIfHelper1 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnTrue, "doIfHelper1");
        await compensator.DoIfAsync(doIfHelper1.TestAsync, doIfHelper1.ExecuteAsync, doIfHelper1.CompensateAsync).ConfigureAwait(false);

        var doIfHelper2 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnTrue, "doIfHelper2");
        await compensator.DoIfAsync(doIfHelper2.TestAsync, doIfHelper2.ExecuteAsync, doIfHelper2.CompensateAsync).ConfigureAwait(false);

        var doIfHelper3 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnFalse, "doIfHelper3");
        await compensator.DoIfAsync(doIfHelper3.TestAsync, doIfHelper3.ExecuteAsync, doIfHelper3.CompensateAsync, doIfTag).ConfigureAwait(false);

        var doIfHelper4 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnTrue, "doIfHelper4");
        await compensator.DoIfAsync(doIfHelper4.TestAsync, doIfHelper4.ExecuteAsync, doIfHelper4.CompensateAsync, doIfTag).ConfigureAwait(false);

        var getTag1 = await compensator.CreateTagAsync("getTag1").ConfigureAwait(false);
        var getTag2 = await compensator.CreateTagAsync("getTag2").ConfigureAwait(false);
        var getTag3 = await compensator.CreateTagAsync("getTag3").ConfigureAwait(false);

        var getHelper1 = new GetHelper("getHelper1");
        await compensator.GetAsync(getHelper1.ExecuteAsync, getHelper1.CompensateAsync).ConfigureAwait(false);

        var getHelper2 = new GetHelper("getHelper2");
        await compensator.GetAsync(getHelper2.ExecuteAsync, getHelper2.CompensateAsync, getTag2).ConfigureAwait(false);

        var getHelper3 = new GetHelper("getHelper3");
        await compensator.GetAsync(getHelper3.ExecuteAsync, getHelper3.CompensateAsync, getTag1).ConfigureAwait(false);

        var foreachHelper1 = new ForeachHelper("foreachHelper1").AddItems(
            new ItemHelper("itemHelper1"),
            new ItemHelper("itemHelper2"),
            new ItemHelper("itemHelper3"));
        await compensator.ForeachAsync(foreachHelper1.Items, foreachHelper1.ExecuteAsync, foreachHelper1.CompensateAsync).ConfigureAwait(false);

        var foreachHelper2 = new ForeachHelper("foreachHelper2").AddItems(
            new ItemHelper("itemHelper4"),
            new ItemHelper("itemHelper5"),
            new ItemHelper("itemHelper6"));
        await compensator.ForeachAsync(foreachHelper2.Items, foreachHelper2.ExecuteAsync, foreachHelper2.CompensateAsync, foreachTag).ConfigureAwait(false);

        var expectedOrderedHelpers = new object[]
        {
            foreachHelper1,
            getHelper1, getTag3, getTag2, getHelper2, getTag1, getHelper3,
            doIfHelper2, doIfHelper1, doIfTag, doIfHelper4,
            doHelper3, doTag, doHelper2, doHelper1,
            foreachHelper2, foreachTag,
        };

        await AssertInternalCompensationOrderAsync(compensator, expectedOrderedHelpers.OfType<HelperBase>().ToArray()).ConfigureAwait(false);

        return expectedOrderedHelpers;
    }

    protected void AssertCompensationException(CompensationException compensationException, bool expectExecutionException = true)
    {
        if (expectExecutionException)
        {
            // assert message
            Assert.Equal(ExpectedMessages.WhileExecutingWhileCompensating(), compensationException.Message);

            // assert while executing
            Assert.Equal(typeof(HelperExecutionException), compensationException.WhileExecuting.GetType());
            Assert.Equal(ExpectedMessages.ExecuteFailed, compensationException.WhileExecuting.Message);

            // assert inner exception
            Assert.Equal(compensationException.WhileExecuting, compensationException.InnerException);
        }
        else
        {
            // assert message
            Assert.Equal(ExpectedMessages.WhileCompensating(), compensationException.Message);

            // assert while executing
            Assert.Null(compensationException.WhileExecuting);

            // assert inner exception
            Assert.Equal(compensationException.WhileCompensating, compensationException.InnerException);
        }

        // assert while compensating
        Assert.Equal(typeof(HelperCompensationException), compensationException.WhileCompensating.GetType());
        Assert.Equal(ExpectedMessages.CompensateFailed, compensationException.WhileCompensating.Message);
    }

    protected void AssertHelpers(params HelperBase[] items)
    {
        foreach (var item in items)
            item.AssertHelper();
    }

    protected async Task AssertInternalCompensationOrderAsync(Compensator compensator, params HelperBase[] expectedOrderedItems)
    {
        // make sure not null
        expectedOrderedItems ??= new HelperBase[0];

        // if specified, expand foreach helpers into items 
        if (expectedOrderedItems.Any(i => i is ForeachHelper))
        {
            await AssertInternalCompensationOrderAsync(compensator, expectedOrderedItems
                .SelectMany(i => i is ForeachHelper foreachHelper
                    ? foreachHelper.GetExpectedCompensationOrder()
                    : new[] { i })
                .ToArray())
                .ConfigureAwait(false);
            return;
        }

        // use reflection to extract private Compenstor._taggedCompensations
        var internalTaggedCompensations = typeof(Compensator)
            .GetField("_taggedCompensations", BindingFlags.Instance | BindingFlags.NonPublic)?
            .GetValue(compensator) as ConcurrentStack<(ConcurrentStack<Func<Task>> Compensations, Tag Tag)>
            ?? throw new Exception("Failed to get internal tagged compensations from compensator");

        // get compensations
        var internalCompensations = internalTaggedCompensations
            .SelectMany(itc => itc.Compensations)
            .ToList();

        // assert expected number of items
        Assert.Equal(expectedOrderedItems.Length, internalCompensations.Count);

        // assert expected items found in specific order
        for (var i = 0; i < expectedOrderedItems.Length; i++)
        {
            var expectedItem = expectedOrderedItems[i];
            var actualCompensation = internalCompensations[i];

            Assert.NotNull(expectedItem);
            Assert.True(await expectedItem.IsExpectedCompensationAsync(actualCompensation).ConfigureAwait(false));
        }
    }
}