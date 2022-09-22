using Compensable.Tests.Helpers;

namespace Compensable.Tests.CompensatorTests
{
    public class AddCompensationAsync : TestBase
    {
        [Fact]
        public async Task CompensationIsNull()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

            Func<Task> compensation = null;

            // act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await compensator.AddCompensationAsync(compensation, tag).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            Assert.Equal(ExpectedMessages.ValueConnotBeNull("compensation"), exception.Message);

            Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

            AssertInternalCompensations(compensator);
        }

        [Fact]
        public async Task CompensationWillFail()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var compensateHelper = new CompensateHelper(throwOnCompensate: true);

            // act
            await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false);

            // assert
            Assert.Equal(CompensatorStatus.Executing, compensator.Status);

            AssertHelpers(compensateHelper);

            AssertInternalCompensations(compensator, compensateHelper, tag);
        }

        [Fact]
        public async Task CompensationWillSucceed()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var compensateHelper = new CompensateHelper();

            // act
            await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false);

            // asssert
            Assert.Equal(CompensatorStatus.Executing, compensator.Status);

            AssertHelpers(compensateHelper);

            AssertInternalCompensations(compensator, compensateHelper, tag);
        }

        [Fact]
        public async Task StatusIsCompensated()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
            var status = CompensatorStatus.Compensated;
            await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

            var compensateHelper = new CompensateHelper();

            // act
            var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
                await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

            AssertHelpers(compensateHelper);
        }

        [Fact(Skip = "Can't arrange correctly right now")]
        public async Task StatusIsCompensating()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
            var status = CompensatorStatus.Compensating;
            await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

            var compensateHelper = new CompensateHelper();

            // act
            var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
                await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

            AssertHelpers(compensateHelper);
        }

        [Fact]
        public async Task StatusIsFailedToCompensate()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);
            var status = CompensatorStatus.FailedToCompensate;
            await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

            var compensateHelper = new CompensateHelper();

            // act
            var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
                await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);

            AssertHelpers(compensateHelper);
        }

        [Fact]
        public async Task TagDoesNotExist()
        {
            // arrange
            var compensator = new Compensator();
            var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var compensateHelper = new CompensateHelper(expectCompensationToBeCalled: false);
            var tag = new Tag();

            // act
            var exception = await Assert.ThrowsAsync<TagNotFoundException>(async () =>
                await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            Assert.Equal(ExpectedMessages.TagNotFound, exception.Message);

            Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

            AssertHelpers(compensateHelper);

            AssertInternalCompensations(compensator);
        }

        [Fact]
        public async Task TagIsNull()
        {
            // arrange
            var compensator = new Compensator();
            var unusedTag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var compensateHelper = new CompensateHelper();
            var tag = default(Tag);

            // act
            await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, tag).ConfigureAwait(false);

            // assert
            Assert.Equal(CompensatorStatus.Executing, compensator.Status);

            AssertHelpers(compensateHelper);

            AssertInternalCompensations(compensator, unusedTag, compensateHelper);
        }
    }
}