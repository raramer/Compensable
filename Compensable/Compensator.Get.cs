using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
	{
        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation, Tag compensateAtTag)
		{
			VerifyCanExecute();

			try
			{
				if (execution == null)
					throw new ArgumentNullException(nameof(execution));

                VerifyTagExists(compensateAtTag);

                await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

				try
				{
					VerifyCanExecute();

					var result = await execution().ConfigureAwait(false);

					if (compensation != null)
                        AddCompensationToStack(async () => await compensation(result).ConfigureAwait(false), compensateAtTag);

                    return result;
				}
                catch
                {
                    await SetStatusAsync(CompensatorStatus.FailedToExecute).ConfigureAwait(false);
                    throw;
                }
                finally
                {
                    _executionLock.Release();
				}
			}
			catch (Exception whileExecuting)
			{
				await CompensateAsync(whileExecuting).ConfigureAwait(false);
				throw;
			}
		}

        #region Execution Overloads
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution)
			=> await GetAsync(execution.Awaitable(), default(Func<TResult, Task>), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution)
            => await GetAsync(execution, default(Func<TResult, Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation Overloads
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation)
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnore<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation)
            => await GetAsync(execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation.Ignore<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation)
            => await GetAsync(execution, compensation.AwaitableIgnore<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation)
            => await GetAsync(execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<Task> compensation)
            => await GetAsync(execution, compensation.Ignore<TResult>(), default(Tag)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation)
            => await GetAsync(execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnore<TResult>(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation.Awaitable(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation.Ignore<TResult>(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation, Tag tag)
            => await GetAsync(execution.Awaitable(), compensation, tag).ConfigureAwait(false);


        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation, Tag tag)
            => await GetAsync(execution, compensation.AwaitableIgnore<TResult>(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation, Tag tag)
            => await GetAsync(execution, compensation.Awaitable(), tag).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<Task> compensation, Tag tag)
            => await GetAsync(execution, compensation.Ignore<TResult>(), tag).ConfigureAwait(false);
        #endregion
    }
}
