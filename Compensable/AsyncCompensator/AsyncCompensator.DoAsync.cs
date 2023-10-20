using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task> execution, Func<Task> compensation, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    await execution().ConfigureAwait(false);

                    if (compensation != null)
                        _compensationStack.AddCompensation(compensation, compensateAtTag);
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>AsyncCompensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task<AsyncCompensation>> execution, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    var executionCompensation = await execution().ConfigureAwait(false);
                    Validate.ExecutionCompensation(executionCompensation);

                    if (executionCompensation.HasCompensation)
                        _compensationStack.AddCompensation(executionCompensation.CompensateAsync, compensateAtTag);
                }).ConfigureAwait(false);
        }

        #region Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Action execution)
            => await DoAsync(execution.Awaitable(), default(Func<Task>), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task> execution)
            => await DoAsync(execution, default(Func<Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Compensable Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Compensation> execution)
            => await DoAsync(execution.AsAsyncCompensation(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>AsyncCompensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task<AsyncCompensation>> execution)
            => await DoAsync(execution, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Action execution, Action compensation)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Action execution, Func<Task> compensation)
            => await DoAsync(execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task> execution, Action compensation)
            => await DoAsync(execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task> execution, Func<Task> compensation)
            => await DoAsync(execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Action execution, Action compensation, Tag compensateAtTag)
            => await DoAsync(execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Action execution, Func<Task> compensation, Tag compensateAtTag)
            => await DoAsync(execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Task> execution, Action compensation, Tag compensateAtTag)
            => await DoAsync(execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion

        #region Compensable Execution + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution.</returns>
        public async Task DoAsync(Func<Compensation> execution, Tag compensateAtTag)
            => await DoAsync(execution.AsAsyncCompensation(), compensateAtTag);
        #endregion
    }
}