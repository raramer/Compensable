using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation, Tag compensateAtTag)
        {
            return await ExecuteAsync(
                validation: () =>
                {
                    ValidateExecution(execution);
                    ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    var result = await execution().ConfigureAwait(false);

                    if (compensation != null)
                        AddCompensationToStack(async () => await compensation(result).ConfigureAwait(false), compensateAtTag);

                    return result;
                }).ConfigureAwait(false);
        }

        #region Execution Overloads
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution)
            => await GetAsync(execution.Awaitable(), default(Func<TResult, Task>), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution)
            => await GetAsync(execution, default(Func<TResult, Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation Overloads
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation)
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation)
            => await GetAsync(execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation.IgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation)
            => await GetAsync(execution, compensation.AwaitableIgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation)
            => await GetAsync(execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<Task> compensation)
            => await GetAsync(execution, compensation.IgnoreParameter<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation)
            => await GetAsync(execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnoreParameter<TResult>(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation.Awaitable(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation.IgnoreParameter<TResult>(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation, tag).ConfigureAwait(false);


        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation, Tag tag)
            => await GetAsync(execution, compensation.AwaitableIgnoreParameter<TResult>(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation, Tag tag)
            => await GetAsync(execution, compensation.Awaitable(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<Task> compensation, Tag tag)
            => await GetAsync(execution, compensation.IgnoreParameter<TResult>(), tag).ConfigureAwait(false);
        #endregion
    }
}
