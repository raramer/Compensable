using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
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
        public async Task AddCompensationAsync(Action compensation)
            => await AddCompensationAsync(compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task AddCompensationAsync(Func<Task> compensation)
            => await AddCompensationAsync(compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Compensation + CompensateAtTag Overload
        public async Task AddCompensationAsync(Action compensation, Tag compensateAtTag)
            => await AddCompensationAsync(compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
