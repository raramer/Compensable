using System.Threading.Tasks;
using System;

namespace Compensable
{
    partial class Compensator
    {
		private async Task ExecuteAsync(Action validation, Func<Task> execution)
		{
			VerifyCanExecute();

			try
			{
				validation?.Invoke();

				await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

				try
				{
					VerifyCanExecute();

					await execution().ConfigureAwait(false);
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
		private async Task ExecuteAsync(Action execution)
			=> await ExecuteAsync(default(Action), execution.Awaitable()).ConfigureAwait(false);
        #endregion

        #region Execution -> TResult Overloads
        private async Task<TResult> ExecuteAsync<TResult>(Func<TResult> execution)
            => await ExecuteAsync<TResult>(default(Action), execution.Awaitable()).ConfigureAwait(false);
        #endregion

        #region Validation + Execution Overloads
        private async Task ExecuteAsync(Action validation, Action execution)
			=> await ExecuteAsync(validation, execution.Awaitable()).ConfigureAwait(false);
		#endregion

		#region Validation + Execution -> TResult Overloads
		private async Task<TResult> ExecuteAsync<TResult>(Action validation, Func<Task<TResult>> execution)
		{
			var result = default(TResult);
			await ExecuteAsync(validation, async () => 
			{ 
				result = await execution().ConfigureAwait(false); 
			}).ConfigureAwait(false);
			return result;
		}
		#endregion
	}
}
