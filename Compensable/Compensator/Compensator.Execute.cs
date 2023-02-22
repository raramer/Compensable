using System;

namespace Compensable
{
    partial class Compensator
    {
        private void Execute(Action validation, Action execution)
        {
            VerifyCanExecute();

            try
            {
                validation?.Invoke();

                _executionLock.Wait(_cancellationToken);

                try
                {
                    VerifyCanExecute();

                    execution();
                }
                catch
                {
                    SetStatus(CompensatorStatus.FailedToExecute);
                    throw;
                }
                finally
                {
                    _executionLock.Release();
                }
            }
            catch (Exception whileExecuting)
            {
                Compensate(whileExecuting);
                throw;
            }
        }

        #region Execution Overloads
        private void Execute(Action execution)
            => Execute(default(Action), execution);
        #endregion

        #region Execution -> TResult Overloads
        private TResult Execute<TResult>(Func<TResult> execution)
            => Execute<TResult>(default(Action), execution);
        #endregion

        #region Validation + Execution -> TResult Overloads
        private TResult Execute<TResult>(Action validation, Func<TResult> execution)
        {
            var result = default(TResult);
            Execute(validation, () => 
            { 
                result = execution(); 
            });
            return result;
        }
        #endregion
    }
}
