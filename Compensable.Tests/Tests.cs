using Xunit.Sdk;

namespace Compensable.Tests
{
    // TODO these tests suck, refactor them
    public class Tests
    {
        [Fact]
        public void Initialized()
        {
            // arrange
            var compensator = new Compensator();

            // assert
            Assert.Equal(Compensator.IsExecuting, compensator.Status);
        }

        [Fact]
        public async Task CompensateNothing()
        {
            // arrange
            var compensator = new Compensator();

            // act
            await compensator.CompensateAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(Compensator.WasCompensated, compensator.Status);
        }

        [Fact]
        public async Task CompensateSucceeds()
        {
            // arrange
            var compensator = new Compensator();

            var doer1 = new Doer();
            await compensator.DoAsync(doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var doer2 = new Doer();
            await compensator.DoAsync(doer2.ExecuteAsync).ConfigureAwait(false);

            var doer3 = new Doer();
            await compensator.DoIfAsync(() => Task.FromResult(true), doer3.ExecuteAsync, doer3.CompensateAsync).ConfigureAwait(false);

            var doer4 = new Doer();
            await compensator.DoIfAsync(() => Task.FromResult(true), doer4.ExecuteAsync).ConfigureAwait(false);

            var doer5 = new Doer();
            await compensator.DoIfAsync(() => Task.FromResult(false), doer5.ExecuteAsync, doer5.CompensateAsync).ConfigureAwait(false);

            var getter1 = new Getter<string>("get1");
            var got1 = await compensator.GetAsync(getter1.ExecuteAsync, getter1.CompensateAsync).ConfigureAwait(false);

            var getter2 = new Getter<string>("get2");
            var got2 = await compensator.GetAsync(getter2.ExecuteAsync).ConfigureAwait(false);

            Assert.Equal(Compensator.IsExecuting, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.True(doer2.ExecutionCalled);
            Assert.True(doer3.ExecutionCalled);
            Assert.True(doer4.ExecutionCalled);
            Assert.False(doer5.ExecutionCalled);
            Assert.True(getter1.ExecutionCalled);
            Assert.True(getter2.ExecutionCalled);

            Assert.False(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.False(doer3.CompensationCalled);
            Assert.False(doer4.CompensationCalled);
            Assert.False(doer5.CompensationCalled);
            Assert.False(getter1.CompensationCalled);
            Assert.False(getter2.CompensationCalled);

            // act
            await compensator.CompensateAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(Compensator.WasCompensated, compensator.Status);

            Assert.True(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.True(doer3.CompensationCalled);
            Assert.False(doer4.CompensationCalled);
            Assert.False(doer5.CompensationCalled);
            Assert.True(getter1.CompensationCalledWithResult);
            Assert.False(getter2.CompensationCalledWithResult);

            var compensationOrder = new Tester[] { doer1, doer2, doer3, doer4, doer5, getter1, getter2 }
                .Where(i => i.CompensationCalledAt != null)
                .OrderBy(i => i.CompensationCalledAt)
                .ToArray();

            Assert.Equal(getter1, compensationOrder[0]);
            Assert.Equal(doer3, compensationOrder[1]);
            Assert.Equal(doer1, compensationOrder[2]);
        }

        [Fact]
        public async Task CompensateFails()
        {
            // arrange
            var compensator = new Compensator();

            var doer1 = new Doer();
            await compensator.DoAsync(doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var doer2 = new Doer();
            await compensator.DoAsync(doer2.ExecuteAsync).ConfigureAwait(false);

            var compensationException = new InvalidOperationException("doer3 failed");
            var doer3 = new Doer { CompensationException = compensationException };
            await compensator.DoIfAsync(() => Task.FromResult(true), doer3.ExecuteAsync, doer3.CompensateAsync).ConfigureAwait(false);

            var doer4 = new Doer();
            await compensator.DoIfAsync(() => Task.FromResult(true), doer4.ExecuteAsync).ConfigureAwait(false);

            var doer5 = new Doer();
            await compensator.DoIfAsync(() => Task.FromResult(false), doer5.ExecuteAsync, doer5.CompensateAsync).ConfigureAwait(false);

            var getter1 = new Getter<string>("get1");
            var got1 = await compensator.GetAsync(getter1.ExecuteAsync, getter1.CompensateAsync).ConfigureAwait(false);

            var getter2 = new Getter<string>("get2");
            var got2 = await compensator.GetAsync(getter2.ExecuteAsync).ConfigureAwait(false);

            Assert.Equal(Compensator.IsExecuting, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.True(doer2.ExecutionCalled);
            Assert.True(doer3.ExecutionCalled);
            Assert.True(doer4.ExecutionCalled);
            Assert.False(doer5.ExecutionCalled);
            Assert.True(getter1.ExecutionCalled);
            Assert.True(getter2.ExecutionCalled);

            Assert.False(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.False(doer3.CompensationCalled);
            Assert.False(doer4.CompensationCalled);
            Assert.False(doer5.CompensationCalled);
            Assert.False(getter1.CompensationCalled);
            Assert.False(getter2.CompensationCalled);

            // act
            var gotException = await Assert.ThrowsAsync<CompensationException>(async () => await compensator.CompensateAsync().ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal(Compensator.FailedToCompensate, compensator.Status);

            Assert.False(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.True(doer3.CompensationCalled);
            Assert.False(doer4.CompensationCalled);
            Assert.False(doer5.CompensationCalled);
            Assert.True(getter1.CompensationCalledWithResult);
            Assert.False(getter2.CompensationCalledWithResult);

            var compensationOrder = new Tester[] { doer1, doer2, doer3, doer4, doer5, getter1, getter2 }
                .Where(i => i.CompensationCalledAt != null)
                .OrderBy(i => i.CompensationCalledAt)
                .ToArray();

            Assert.Equal(getter1, compensationOrder[0]);
            Assert.Equal(doer3, compensationOrder[1]);
        }



        [Fact]
        public async Task DoExecutionFails()
        {
            // arrange
            var compensator = new Compensator();

            var doer1 = new Doer();
            await compensator.DoAsync(doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var executionException = new InvalidOperationException("execution failed");
            var doer2 = new Doer() { ExecutionException = executionException };

            // act
            var gotException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await compensator.DoAsync(doer2.ExecuteAsync, doer2.CompensateAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal(executionException.Message, gotException.Message);

            Assert.Equal(Compensator.WasCompensated, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.True(doer2.ExecutionCalled);
            Assert.True(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
        }

        [Fact]
        public async Task DoCompensationFails()
        {
            // arrange
            var compensator = new Compensator();

            var compensationException = new InvalidOperationException("compensation failed");
            var doer1 = new Doer { CompensationException = compensationException };
            await compensator.DoAsync(doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var executionException = new InvalidOperationException("execution failed");
            var doer2 = new Doer { ExecutionException = executionException };

            // act
            var gotException = await Assert.ThrowsAsync<CompensationException>(async () => await compensator.DoAsync(doer2.ExecuteAsync, doer2.CompensateAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal($"While executing: {executionException.Message}{Environment.NewLine}While compensating: {compensationException.Message}", gotException.Message);
            Assert.Equal(compensationException.Message, gotException.WhileCompensating.Message);
            Assert.Equal(executionException.Message, gotException.WhileExecuting.Message);

            Assert.Equal(Compensator.FailedToCompensate, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.True(doer2.ExecutionCalled);
            Assert.True(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
        }

        [Fact]
        public async Task DoIfTestFails()
        {
            // arrange
            var compensator = new Compensator();

            var test1 = () => Task.FromResult(true);
            var doer1 = new Doer();
            await compensator.DoIfAsync(test1, doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var test2 = () => Task.FromResult(false);
            var doer2 = new Doer();
            await compensator.DoIfAsync(test2, doer2.ExecuteAsync, doer2.CompensateAsync).ConfigureAwait(false);

            var testException = new InvalidOperationException("test failed");
            Func<Task<bool>> test3 = () => throw testException;
            var doer3 = new Doer();

            // act
            var gotException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await compensator.DoIfAsync(test3, doer3.ExecuteAsync, doer3.CompensateAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal(testException.Message, gotException.Message);

            Assert.Equal(Compensator.WasCompensated, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.False(doer2.ExecutionCalled);
            Assert.False(doer3.ExecutionCalled);
            Assert.True(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.False(doer3.CompensationCalled);
        }

        [Fact]
        public async Task DoIfExecutionFails()
        {
            // arrange
            var compensator = new Compensator();

            var test1 = () => Task.FromResult(true);
            var doer1 = new Doer();
            await compensator.DoIfAsync(test1, doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var test2 = () => Task.FromResult(false);
            var doer2 = new Doer();
            await compensator.DoIfAsync(test2, doer2.ExecuteAsync, doer2.CompensateAsync).ConfigureAwait(false);

            var test3 = () => Task.FromResult(true);
            var executionException = new InvalidOperationException("execution failed");
            var doer3 = new Doer() { ExecutionException = executionException };

            // act
            var gotException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await compensator.DoIfAsync(test3, doer3.ExecuteAsync, doer3.CompensateAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal(executionException.Message, gotException.Message);

            Assert.Equal(Compensator.WasCompensated, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.False(doer2.ExecutionCalled);
            Assert.True(doer3.ExecutionCalled);
            Assert.True(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.False(doer3.CompensationCalled);
        }

        [Fact]
        public async Task DoIfCompensationFails()
        {
            // arrange
            var compensator = new Compensator();

            var test1 = () => Task.FromResult(true);
            var compensationException = new InvalidOperationException("compensation failed");
            var doer1 = new Doer { CompensationException = compensationException };
            await compensator.DoIfAsync(test1, doer1.ExecuteAsync, doer1.CompensateAsync).ConfigureAwait(false);

            var test2 = () => Task.FromResult(false);
            var doer2 = new Doer();
            await compensator.DoIfAsync(test2, doer2.ExecuteAsync, doer2.CompensateAsync).ConfigureAwait(false);

            var test3 = () => Task.FromResult(true);
            var executionException = new InvalidOperationException("execution failed");
            var doer3 = new Doer { ExecutionException = executionException };

            // act
            var gotException = await Assert.ThrowsAsync<CompensationException>(async () => await compensator.DoIfAsync(test3, doer3.ExecuteAsync, doer3.CompensateAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal($"While executing: {executionException.Message}{Environment.NewLine}While compensating: {compensationException.Message}", gotException.Message);
            Assert.Equal(compensationException.Message, gotException.WhileCompensating.Message);
            Assert.Equal(executionException.Message, gotException.WhileExecuting.Message);

            Assert.Equal(Compensator.FailedToCompensate, compensator.Status);

            Assert.True(doer1.ExecutionCalled);
            Assert.False(doer2.ExecutionCalled);
            Assert.True(doer3.ExecutionCalled);
            Assert.True(doer1.CompensationCalled);
            Assert.False(doer2.CompensationCalled);
            Assert.False(doer3.CompensationCalled);
        }

        [Fact]
        public async Task GetExecutionFails()
        {
            // arrange
            var compensator = new Compensator();

            var getter1 = new Doer();
            await compensator.DoAsync(getter1.ExecuteAsync, getter1.CompensateAsync).ConfigureAwait(false);

            var executionException = new InvalidOperationException("execution failed");
            var getter2 = new Doer() { ExecutionException = executionException };

            // act
            var gotException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await compensator.DoAsync(getter2.ExecuteAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal(executionException.Message, gotException.Message);

            Assert.Equal(Compensator.WasCompensated, compensator.Status);

            Assert.True(getter1.ExecutionCalled);
            Assert.True(getter2.ExecutionCalled);
            Assert.True(getter1.CompensationCalled);
            Assert.False(getter2.CompensationCalled);
        }

        [Fact]
        public async Task GetCompensationFails()
        {
            // arrange
            var compensator = new Compensator();

            var compensationException = new InvalidOperationException("compensation failed");
            var getter1 = new Doer { CompensationException = compensationException };
            await compensator.DoAsync(getter1.ExecuteAsync, getter1.CompensateAsync).ConfigureAwait(false);

            var executionException = new InvalidOperationException("execution failed");
            var getter2 = new Doer { ExecutionException = executionException };

            // act
            var gotException = await Assert.ThrowsAsync<CompensationException>(async () => await compensator.DoAsync(getter2.ExecuteAsync).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal($"While executing: {executionException.Message}{Environment.NewLine}While compensating: {compensationException.Message}", gotException.Message);
            Assert.Equal(compensationException.Message, gotException.WhileCompensating.Message);
            Assert.Equal(executionException.Message, gotException.WhileExecuting.Message);

            Assert.Equal(Compensator.FailedToCompensate, compensator.Status);

            Assert.True(getter1.ExecutionCalled);
            Assert.True(getter2.ExecutionCalled);
            Assert.True(getter1.CompensationCalled);
            Assert.False(getter2.CompensationCalled);
        }

        private class Tester 
        { 
            public long? CompensationCalledAt { get; protected set; }
            public bool CompensationCalled { get; protected set; }
            public Exception? CompensationException { get; init; }
            public bool ExecutionCalled { get; protected set; }
            public Exception? ExecutionException { get; init; }
        }

        private class Doer : Tester
        {
            public async Task CompensateAsync()
            {
                await Task.Delay(1).ConfigureAwait(false);
                CompensationCalled = true;
                CompensationCalledAt = DateTime.UtcNow.Ticks;
                if (CompensationException != null)
                    throw CompensationException;
            }

            public async Task ExecuteAsync()
            {
                await Task.Delay(1).ConfigureAwait(false);
                ExecutionCalled = true;
                if (ExecutionException != null)
                    throw ExecutionException;
            }
        }

        private class Getter<TResult> : Tester
        {
            public bool CompensationCalledWithResult { get; private set; }
            public TResult Result { get; }

            public Getter(TResult result)
            {
                Result = result;
            }

            public async Task CompensateAsync(TResult result)
            {
                await Task.Delay(1).ConfigureAwait(false);
                CompensationCalled = true;
                CompensationCalledAt = DateTime.UtcNow.Ticks;
                CompensationCalledWithResult = Object.Equals(result, Result);
                if (CompensationException != null)
                    throw CompensationException;
            }

            public async Task<TResult> ExecuteAsync()
            {
                await Task.Delay(1).ConfigureAwait(false);
                ExecutionCalled = true;
                if (ExecutionException != null)
                    throw ExecutionException;
                return Result;
            }
        }
    }
}