using Compensable.Tests.Helpers;

namespace Compensable.Tests.CompensatorTests
{
    public class CompensateAsync : TestBase
    {
        [Fact]
        public async Task CompensationFails()
        {
            // arrange
            var compensator = new Compensator();

            var arrangedCompensations = await ArrangeCompensationsAsync(compensator).ConfigureAwait(false);
            var arrangedTag = arrangedCompensations.OfType<Tag>().Skip(2).First();

            var compensateHelper = new CompensateHelper(throwOnCompensate: true, expectCompensationToBeCalled: true);
            await compensator.AddCompensationAsync(compensateHelper.CompensateAsync, arrangedTag).ConfigureAwait(false);
            
            // act
            var exception = await Assert.ThrowsAsync<CompensationException>(async () =>
                await compensator.CompensateAsync().ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            AssertCompensationException(exception, expectExecutionException: false);

            Assert.Equal(CompensatorStatus.FailedToCompensate, compensator.Status);

            AssertInternalCompensations(compensator, arrangedCompensations
                .TakeWhile(t => t != arrangedTag)
                .Append(compensateHelper)
                .ToArray());
        }

        [Fact]
        public async Task CompensationSucceeds()
        {
            // arrange
            var compensator = new Compensator();

            var expectedCompensations = await ArrangeCompensationsAsync(compensator).ConfigureAwait(false);

            // act
            await compensator.CompensateAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

            AssertInternalCompensations(compensator);
        }

        [Fact]
        public async Task NothingToCompensate()
        {
            // arrange
            var compensator = new Compensator();

            // act
            await compensator.CompensateAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

            AssertInternalCompensations(compensator);
        }

        [Fact]
        public async Task StatusIsCompensated()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var status = CompensatorStatus.Compensated;
            await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

            // act
            await compensator.CompensateAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

            AssertInternalCompensations(compensator);
        }

        [Fact(Skip = "Can't arrange correctly right now")]
        public async Task StatusIsCompensating()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var status = CompensatorStatus.Compensating;
            await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

            // act
            await compensator.CompensateAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(CompensatorStatus.Compensated, compensator.Status);

            AssertInternalCompensations(compensator);
        }

        [Fact]
        public async Task StatusIsFailedToCompensate()
        {
            // arrange
            var compensator = new Compensator();
            var tag = await compensator.CreateTagAsync().ConfigureAwait(false);

            var status = CompensatorStatus.FailedToCompensate;
            await ArrangeStatusAsync(compensator, status).ConfigureAwait(false);

            // act
            var exception = await Assert.ThrowsAsync<CompensatorStatusException>(async () =>
                await compensator.CompensateAsync().ConfigureAwait(false)
            ).ConfigureAwait(false);

            // assert
            Assert.Equal(ExpectedMessages.CompensatorStatusIs(status), exception.Message);
        }
    }
}