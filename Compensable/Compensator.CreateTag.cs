using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Creates a new Tag in the compensation stack with the specified label
        /// </summary>
        /// <param name="label">A label to view while debugging; there is no guarantee of uniqueness.  If null, a random value will be generated</param>
        /// <returns>A new Tag with the specified label.</returns>
        public async Task<Tag> CreateTagAsync(string label)
        {
            ValidateStatusIsExecuting();

            try
            {
                await _executionLock.WaitAsync().ConfigureAwait(false);

                try
                {
                    ValidateStatusIsExecuting();

                    var tag = new Tag(label);

                    _compensations.Add((null, tag));

                    return tag;
                }
                finally
                {
                    // TODO if an exception occurs, we will compensate but there's no lock between this finally and the outer catch
                    _executionLock.Release();
                }
            }
            catch (Exception whileExecuting)
            {
                await CompensateAsync(whileExecuting).ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// Creates a new Tag in the compensation stack.
        /// </summary>
        /// <returns>A new Tag.</returns>
        public async Task<Tag> CreateTagAsync()
            => await CreateTagAsync(default(string)).ConfigureAwait(false);
    }
}
