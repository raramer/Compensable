﻿using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        internal async Task<Tag> CreateTagAsync(string label)
        {
            VerifyCanExecute();

            try
            {
                await _executionLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

                try
                {
                    VerifyCanExecute();

                    return AddTagToStack(new Tag(label));
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

        #region Parameterless Overload
        public async Task<Tag> CreateTagAsync()
            => await CreateTagAsync(default(string)).ConfigureAwait(false);
        #endregion
    }
}