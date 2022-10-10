using System;
using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task AddCompensationAsync(Func<Task> compensation, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    ValidateCompensation(compensation);
                    ValidateTag(compensateAtTag);
                },
                execution: () => AddCompensationToStack(compensation, compensateAtTag)
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
