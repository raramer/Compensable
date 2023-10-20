using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    Validate.Test(test);
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    if (await test().ConfigureAwait(false))
                    {
                        await execution().ConfigureAwait(false);

                        if (compensation != null)
                            _compensationStack.AddCompensation(compensation, compensateAtTag);
                    }
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>AsyncCompensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task<AsyncCompensation>> execution, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    Validate.Test(test);
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    if (await test().ConfigureAwait(false))
                    {
                        var executionCompensation = await execution().ConfigureAwait(false);
                        Validate.ExecutionCompensation(executionCompensation);

                        if (executionCompensation.HasCompensation)
                            _compensationStack.AddCompensation(executionCompensation.CompensateAsync, compensateAtTag);
                    }
                }).ConfigureAwait(false);
        }

        #region Test + Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Action execution)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task> execution)
            => await DoIfAsync(() => Task.FromResult(test), execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="execution"></param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Action execution)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task> execution)
            => await DoIfAsync(test.Awaitable(), execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Action execution)
            => await DoIfAsync(test, execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution)
            => await DoIfAsync(test, execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Test + Compensable Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Compensation> execution)
            => await DoIfAsync(() => Task.FromResult(test), execution.AsAsyncCompensation(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>AsyncCompensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task<AsyncCompensation>> execution)
            => await DoIfAsync(() => Task.FromResult(test), execution, default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="execution"></param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Compensation> execution)
            => await DoIfAsync(test.Awaitable(), execution.AsAsyncCompensation(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>AsyncCompensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task<AsyncCompensation>> execution)
            => await DoIfAsync(test.Awaitable(), execution, default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Compensation> execution)
            => await DoIfAsync(test, execution.AsAsyncCompensation(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>AsyncCompensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task<AsyncCompensation>> execution)
            => await DoIfAsync(test, execution, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Test + Execution + Compensation Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Action execution, Action compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Action execution, Func<Task> compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task> execution, Action compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task> execution, Func<Task> compensation)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation, default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Action execution, Action compensation)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Action execution, Func<Task> compensation)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Action compensation)
            => await DoIfAsync(test.Awaitable(), execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Func<Task> compensation)
            => await DoIfAsync(test.Awaitable(), execution, compensation, default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Action compensation)
            => await DoIfAsync(test, execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Func<Task> compensation)
            => await DoIfAsync(test, execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Action compensation)
            => await DoIfAsync(test, execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Func<Task> compensation)
            => await DoIfAsync(test, execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Test + Execution + Compensation + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Action execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensation, compensateAtTag).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Action execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution, compensation, compensateAtTag).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test, execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoIfAsync(test, execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoIfAsync(test, execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion

        #region Test + Compensable Execution + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Compensation> execution, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution.AsAsyncCompensation(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>AsyncCompensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(bool test, Func<Task<AsyncCompensation>> execution, Tag compensateAtTag)
            => await DoIfAsync(() => Task.FromResult(test), execution, compensateAtTag).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="execution"></param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Compensation> execution, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution.AsAsyncCompensation(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>AsyncCompensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<bool> test, Func<Task<AsyncCompensation>> execution, Tag compensateAtTag)
            => await DoIfAsync(test.Awaitable(), execution, compensateAtTag).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents evaluating the test and running the execution.</returns>
        public async Task DoIfAsync(Func<Task<bool>> test, Func<Compensation> execution, Tag compensateAtTag)
            => await DoIfAsync(test, execution.AsAsyncCompensation(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}