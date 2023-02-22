using Compensable.Tests.Helpers.Bases;
using System.Collections.Concurrent;
using System.Reflection;

namespace Compensable.Tests.CompensatorTests;

public abstract class TestBase
{
    protected void ArrangeStatus(Compensator compensator, CompensatorStatus status)
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
                compensator.Compensate();
                break;

            case CompensatorStatus.FailedToCompensate:
                compensator.AddCompensation(() => throw new InvalidOperationException("Failed to compensate"));
                try
                {
                    compensator.Compensate();
                }
                catch
                {
                    // Ignore exceptions
                }
                break;
        }
    }

    protected object[] ArrangeTagsAndCompensations(Compensator compensator)
    {
        var foreachTag = compensator.CreateTag("foreachTag");

        var doTag = compensator.CreateTag("doTag");

        var doHelper1 = new DoHelper("doHelper1");
        compensator.Do(doHelper1.Execute, doHelper1.Compensate, doTag);

        var doHelper2 = new DoHelper("doHelper2");
        compensator.Do(doHelper2.Execute, doHelper2.Compensate, doTag);

        var doHelper3 = new DoHelper("doHelper3");
        compensator.Do(doHelper3.Execute, doHelper3.Compensate);

        var doIfTag = compensator.CreateTag("doIfTag");

        var doIfHelper1 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnTrue, "doIfHelper1");
        compensator.DoIf(doIfHelper1.Test, doIfHelper1.Execute, doIfHelper1.Compensate);

        var doIfHelper2 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnTrue, "doIfHelper2");
        compensator.DoIf(doIfHelper2.Test, doIfHelper2.Execute, doIfHelper2.Compensate);

        var doIfHelper3 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnFalse, "doIfHelper3");
        compensator.DoIf(doIfHelper3.Test, doIfHelper3.Execute, doIfHelper3.Compensate, doIfTag);

        var doIfHelper4 = new DoIfHelper(TestOptions.ShouldBeCalledAndReturnTrue, "doIfHelper4");
        compensator.DoIf(doIfHelper4.Test, doIfHelper4.Execute, doIfHelper4.Compensate, doIfTag);

        var getTag1 = compensator.CreateTag("getTag1");
        var getTag2 = compensator.CreateTag("getTag2");
        var getTag3 = compensator.CreateTag("getTag3");

        var getHelper1 = new GetHelper("getHelper1");
        compensator.Get(getHelper1.Execute, getHelper1.Compensate);

        var getHelper2 = new GetHelper("getHelper2");
        compensator.Get(getHelper2.Execute, getHelper2.Compensate, getTag2);

        var getHelper3 = new GetHelper("getHelper3");
        compensator.Get(getHelper3.Execute, getHelper3.Compensate, getTag1);

        var foreachHelper1 = new ForeachHelper("foreachHelper1").AddItems(
            new ItemHelper("itemHelper1"),
            new ItemHelper("itemHelper2"),
            new ItemHelper("itemHelper3"));
        compensator.Foreach(foreachHelper1.Items, foreachHelper1.Execute, foreachHelper1.Compensate);

        var foreachHelper2 = new ForeachHelper("foreachHelper2").AddItems(
            new ItemHelper("itemHelper4"),
            new ItemHelper("itemHelper5"),
            new ItemHelper("itemHelper6"));
        compensator.Foreach(foreachHelper2.Items, foreachHelper2.Execute, foreachHelper2.Compensate, foreachTag);

        var expectedOrderedHelpers = new object[]
        {
            foreachHelper1,
            getHelper1, getTag3, getTag2, getHelper2, getTag1, getHelper3,
            doIfHelper2, doIfHelper1, doIfTag, doIfHelper4,
            doHelper3, doTag, doHelper2, doHelper1,
            foreachHelper2, foreachTag,
        };

        AssertInternalCompensationOrder(compensator, expectedOrderedHelpers.OfType<HelperBase>().ToArray());

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

    protected void AssertInternalCompensationOrder(Compensator compensator, params HelperBase[] expectedOrderedItems)
    {
        // make sure not null
        expectedOrderedItems ??= new HelperBase[0];

        // if specified, expand foreach helpers into items 
        if (expectedOrderedItems.Any(i => i is ForeachHelper))
        {
            AssertInternalCompensationOrder(compensator, expectedOrderedItems
                .SelectMany(i => i is ForeachHelper foreachHelper
                    ? foreachHelper.GetExpectedCompensationOrder()
                    : new[] { i })
                .ToArray())
                ;
            return;
        }

        // use reflection to extract tagged compensations
        var compensationStack = typeof(Compensator)
            .GetField("_compensationStack", BindingFlags.Instance | BindingFlags.NonPublic)?
            .GetValue(compensator) as CompensationStack<Action>
            ?? throw new Exception("Failed to get internal compensation stack from compensator");

        var internalTaggedCompensations = typeof(CompensationStack<Action>)
            .GetField("_taggedCompensations", BindingFlags.Instance | BindingFlags.NonPublic)?
            .GetValue(compensationStack) as ConcurrentStack<(ConcurrentStack<Action> Compensations, Tag Tag)>
            ?? throw new Exception("Failed to get internal tagged compensations from compensation stack");

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
            Assert.True(expectedItem.IsExpectedCompensation(actualCompensation));
        }
    }
}