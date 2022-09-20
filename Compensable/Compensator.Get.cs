using System;
using System.Threading.Tasks;

namespace Compensable
{
	partial class Compensator
	{
		public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Func<TResult, Task> compensation)
		{
			VerifyStatusIsExecuting();

			try
			{
				if (execution == null)
					throw new ArgumentException($"{nameof(execution)} is required.");

				await _executionLock.WaitAsync().ConfigureAwait(false);

				try
				{
					VerifyStatusIsExecuting();

					var result = await execution().ConfigureAwait(false);

					if (compensation != null)
						_compensations.Add(async () => await compensation(result).ConfigureAwait(false));

					return result;
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

		public async Task<TResult> GetAsync<TResult>(Func<TResult> execution)
			=> await GetAsync(execution.Awaitable(), default(Func<TResult, Task>)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action compensation)
            => await GetAsync(execution.Awaitable(), compensation.AwaitableIgnore<TResult>()).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Action<TResult> compensation)
			=> await GetAsync(execution.Awaitable(), compensation.Awaitable()).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<Task> compensation)
            => await GetAsync(execution.Awaitable(), compensation.Ignore<TResult>()).ConfigureAwait(false);
        
		public async Task<TResult> GetAsync<TResult>(Func<TResult> execution, Func<TResult, Task> compensation)
			=> await GetAsync(execution.Awaitable(), compensation).ConfigureAwait(false);

		public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution)
			=> await GetAsync(execution, default(Func<TResult, Task>)).ConfigureAwait(false);

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action compensation)
            => await GetAsync(execution, compensation.AwaitableIgnore<TResult>()).ConfigureAwait(false);
    
		public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> execution, Action<TResult> compensation)
        => await GetAsync(execution, compensation.Awaitable()).ConfigureAwait(false);
    }
}
