using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation, Tag compensateAtTag)
        {
            return await ExecuteAsync(
                validation: () =>
                {
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    var result = await execution().ConfigureAwait(false);

                    if (compensation != null)
                        _compensationStack.AddCompensation(async () => await compensation(result).ConfigureAwait(false), compensateAtTag);

                    return result;
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<AsyncCompensation<TResult>>> execution, Tag compensateAtTag)
        {
            return await ExecuteAsync(
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

                    return executionCompensation.Result;
                }).ConfigureAwait(false);
        }

        #region Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution)
            => await GetAsync(execution.Awaitable(), default(Func<TResult, Task>), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution)
            => await GetAsync(execution, default(Func<TResult, Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Compensable Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Compensation<TResult>> execution)
            => await GetAsync(execution.AsAsyncCompensation(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<AsyncCompensation<TResult>>> execution)
            => await GetAsync(execution, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation)
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation)
            => await GetAsync(execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation.IgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation)
            => await GetAsync(execution, compensation.AwaitableIgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation)
            => await GetAsync(execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<Task> compensation)
            => await GetAsync(execution, compensation.IgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation)
            => await GetAsync(execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnoreParameter<TResult>(), tag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution.Awaitable(), compensation.Awaitable(), tag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution.Awaitable(), compensation.IgnoreParameter<TResult>(), tag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution.Awaitable(), compensation, tag).ConfigureAwait(false);


        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution, compensation.AwaitableIgnoreParameter<TResult>(), tag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution, compensation.Awaitable(), tag).ConfigureAwait(false);

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<Task> compensation, Tag tag) // TODO change this to "compensateAtTag" in next major version
            => await GetAsync(execution, compensation.IgnoreParameter<TResult>(), tag).ConfigureAwait(false);
        #endregion

        #region Compensable Execution + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>A task that represents running the execution and returning the result.</returns>
        public async Task<TResult> GetAsync<TResult>(Func<Compensation<TResult>> execution, Tag compensateAtTag)
            => await GetAsync(execution.AsAsyncCompensation(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
