using Compensable.Tests.Helpers;
using System.Reflection;

namespace Compensable.Tests.CompensatorTests
{
    public abstract class TestBase
    {
        protected async Task ArrangeStatusAsync(Compensator compensator, CompensatorStatus status)
        {
            switch (status)
            {
                case CompensatorStatus.Compensating:
                case CompensatorStatus.Compensated:
                    await compensator.CompensateAsync().ConfigureAwait(false);
                    break;

                case CompensatorStatus.FailedToCompensate:
                    await compensator.AddCompensationAsync(() => throw new InvalidOperationException("Failed to compensate")).ConfigureAwait(false);
                    try { await compensator.CompensateAsync().ConfigureAwait(false); }
                    catch { }
                    break;
            }
        }

        protected async Task<object[]> ArrangeCompensationsAsync(Compensator compensator)
        {
            var doTag = await compensator.CreateTagAsync("doTag").ConfigureAwait(false);

            var doHelper1 = new DoHelper("doHelper1");
            await compensator.DoAsync(doHelper1.ExecuteAsync, doHelper1.CompensateAsync, doTag).ConfigureAwait(false);

            var doHelper2 = new DoHelper("doHelper2");
            await compensator.DoAsync(doHelper2.ExecuteAsync, doHelper2.CompensateAsync, doTag).ConfigureAwait(false);

            var doHelper3 = new DoHelper("doHelper3");
            await compensator.DoAsync(doHelper3.ExecuteAsync, doHelper3.CompensateAsync).ConfigureAwait(false);


            var doIfTag = await compensator.CreateTagAsync("doIfTag").ConfigureAwait(false);

            var doIfHelper1 = new DoIfHelper(testResult: true, "doIfHelper1");
            await compensator.DoIfAsync(doIfHelper1.TestAsync, doIfHelper1.ExecuteAsync, doIfHelper1.CompensateAsync).ConfigureAwait(false);

            var doIfHelper2 = new DoIfHelper(testResult: true, "doIfHelper2");
            await compensator.DoIfAsync(doIfHelper2.TestAsync, doIfHelper2.ExecuteAsync, doIfHelper2.CompensateAsync).ConfigureAwait(false);

            var doIfHelper3 = new DoIfHelper(testResult: false, "doIfHelper3");
            await compensator.DoIfAsync(doIfHelper3.TestAsync, doIfHelper3.ExecuteAsync, doIfHelper3.CompensateAsync, doIfTag).ConfigureAwait(false);

            var doIfHelper4 = new DoIfHelper(testResult: true, "doIfHelper4");
            await compensator.DoIfAsync(doIfHelper4.TestAsync, doIfHelper4.ExecuteAsync, doIfHelper4.CompensateAsync, doIfTag).ConfigureAwait(false);

            var getTag1 = await compensator.CreateTagAsync("getTag1").ConfigureAwait(false);
            var getTag2 = await compensator.CreateTagAsync("getTag2").ConfigureAwait(false);
            var getTag3 = await compensator.CreateTagAsync("getTag3").ConfigureAwait(false);

            var getHelper1 = new DoHelper("getHelper1");
            await compensator.DoAsync(getHelper1.ExecuteAsync, getHelper1.CompensateAsync).ConfigureAwait(false);

            var getHelper2 = new DoHelper("getHelper2");
            await compensator.DoAsync(getHelper2.ExecuteAsync, getHelper2.CompensateAsync, getTag2).ConfigureAwait(false);

            var getHelper3 = new DoHelper("getHelper3");
            await compensator.DoAsync(getHelper3.ExecuteAsync, getHelper3.CompensateAsync, getTag1).ConfigureAwait(false);

            var expectedCompensations = new object[]
            {
                doHelper1, doHelper2, doTag, doHelper3,
                doIfHelper4, doIfTag, doIfHelper1, doIfHelper2,
                getHelper3, getTag1, getHelper2, getTag2, getTag3, getHelper1,
            };

            AssertInternalCompensations(compensator, expectedCompensations);

            return expectedCompensations;
        }

        protected void AssertCompensationException(CompensationException compensationException, bool expectExecutionException = true)
        {
            if (expectExecutionException)
            {
                // assert message
                Assert.Equal(ExpectedMessages.WhileExecutingWhileCompensating, compensationException.Message);
                
                // assert while executing
                Assert.Equal(typeof(HelperExecutionException), compensationException.WhileExecuting.GetType());
                Assert.Equal(ExpectedMessages.ExecuteFailed, compensationException.WhileExecuting.Message);

                // assert inner exception
                Assert.Equal(compensationException.WhileExecuting, compensationException.InnerException);
            }
            else
            {
                // assert message
                Assert.Equal(ExpectedMessages.WhileCompensating, compensationException.Message);

                // assert while executing
                Assert.Null(compensationException.WhileExecuting);

                // assert inner exception
                Assert.Equal(compensationException.WhileCompensating, compensationException.InnerException);
            }

            // assert while compensating
            Assert.Equal(typeof(HelperCompensationException), compensationException.WhileCompensating.GetType());
            Assert.Equal(ExpectedMessages.CompensateFailed, compensationException.WhileCompensating.Message);
        }

        protected void AssertCompensated(params CompensateHelperBase[] expectedOrderedItems)
        {
            Assert.True(expectedOrderedItems.All(item => item.CompensationCalled && item.CompensationCalledAt != null));

            var actualOrder = expectedOrderedItems.OrderByDescending(helper => helper.CompensationCalledAt).ToArray();

            for (var i = 0; i < expectedOrderedItems.Length; i++)
            {
                var expectedItem = expectedOrderedItems[i];
                var actualItem = actualOrder[i];

                Assert.Equal(expectedItem, actualItem);

                if (expectedItem is ExecuteCompensateHelperBase executeCompensateHelper)
                    Assert.True(executeCompensateHelper.ExecutionCalled);

                if (expectedItem is DoIfHelper doIfHelper)
                    Assert.True(doIfHelper.TestCalled);
            }
        }

        protected void AssertHelpers(params CompensateHelperBase[] items)
        {
            foreach (var item in items)
                item.AssertHelper();
        }

        protected void AssertInternalCompensations(Compensator compensator, params object[] expectedOrderedItems)
        {
            // use reflection to extract private Compenstor._compensations
            var internalCompensations = typeof(Compensator)
                .GetField("_compensations", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(compensator) as List<(Func<Task> Compensate, Tag Tag)>
                ?? throw new Exception("Failed to get internal compensations from compensator");

            // assert expected number of items
            Assert.Equal(expectedOrderedItems.Length, internalCompensations.Count);

            // assert expected items found in specific order
            for (var i = 0; i < expectedOrderedItems.Length; i++)
            {
                var expectedItem = expectedOrderedItems[i];
                var actualItem = internalCompensations[i];

                if (expectedItem is GetHelper getHelper)
                {
                    // actualItem.Compensate is a parameterless function that wraps expectedItem.Compensate. We don't want to do weird stuff only for
                    // testing purposes. Instead call actualItem.Compensate and check GetHelper.CompensationCalled.
                    Assert.NotNull(actualItem.Compensate);
                    Assert.Null(actualItem.Tag);

                    getHelper.ResetCompensationCalled();
                    try { actualItem.Compensate().GetAwaiter().GetResult(); }
                    catch { }
                    Assert.True(getHelper.CompensationCalled);
                }
                else if (expectedItem is DoIfHelper doIfHelper)
                {
                    Assert.Equal(doIfHelper.CompensateAsync, actualItem.Compensate);
                    Assert.Null(actualItem.Tag);
                }
                else if (expectedItem is DoHelper doHelper)
                {
                    Assert.Equal(doHelper.CompensateAsync, actualItem.Compensate);
                    Assert.Null(actualItem.Tag);
                }
                else if (expectedItem is CompensateHelper compensateHelper)
                {
                    Assert.Equal(compensateHelper.CompensateAsync, actualItem.Compensate);
                    Assert.Null(actualItem.Tag);
                }
                else if (expectedItem is Tag)
                {
                    Assert.Null(actualItem.Compensate);
                    Assert.Equal(expectedItem, actualItem.Tag);
                }
                else
                {
                    throw new Exception("Not a supported expected item");
                }
            }
        }
    }
}