using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        /// <summary>
        /// Adds a <i>compensation</i> to a tagged position in the compensation stack without requiring a successfully completed execution.
        /// </summary>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">An tagged position in the compensation stack.</param>
        /// <returns>A task that represents adding the compensation.</returns>
        public async Task AddCompensationAsync(Func<Task> compensation, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    Validate.Compensation(compensation);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () => _compensationStack.AddCompensation(compensation, compensateAtTag)
            ).ConfigureAwait(false);
        }

        #region Compensation Overloads
        /// <summary>
        /// Adds a <i>compensation</i> to the compensation stack without requiring a successfully completed execution.
        /// </summary>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents adding the compensation.</returns>
        public async Task AddCompensationAsync(Action compensation)
            => await AddCompensationAsync(compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        /// <summary>
        /// Adds a <i>compensation</i> to the compensation stack without requiring a successfully completed execution.
        /// </summary>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>A task that represents adding the compensation.</returns>
        public async Task AddCompensationAsync(Func<Task> compensation)
            => await AddCompensationAsync(compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Compensation + CompensateAtTag Overload
        /// <summary>
        /// Adds a <i>compensation</i> to a tagged position in the compensation stack without requiring a successfully completed execution.
        /// </summary>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">An tagged position in the compensation stack.</param>
        /// <returns>A task that represents adding the compensation.</returns>
        public async Task AddCompensationAsync(Action compensation, Tag compensateAtTag)
            => await AddCompensationAsync(compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
