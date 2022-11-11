using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Func<TItem, Task> execution, Func<TItem, Task> compensation, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    Validate.Items(items);
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: async () =>
                {
                    // TODO do we need create a foreach specific Tag to guarantee all foreach item compensations are grouped?
                    foreach (var next in items)
                    {
                        // store item to local variable as the value of next is updated by the enumerator
                        var item = next;

                        await execution(item).ConfigureAwait(false);

                        if (compensation != null)
                            _compensationStack.AddCompensation(async () => await compensation(item).ConfigureAwait(false), compensateAtTag);
                    }
                }).ConfigureAwait(false);
        }

        #region Items + Execution Overloads
        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Action<TItem> execution)
            => await ForeachAsync<TItem>(items, execution.Awaitable(), default(Func<TItem, Task>), default(Tag)).ConfigureAwait(false);

        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Func<TItem, Task> execution)
            => await ForeachAsync<TItem>(items, execution, default(Func<TItem, Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Items + Execution + Compensation Overloads
        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Action<TItem> compensation)
            => await ForeachAsync<TItem>(items, execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Func<TItem, Task> compensation)
            => await ForeachAsync<TItem>(items, execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Func<TItem, Task> execution, Action<TItem> compensation)
            => await ForeachAsync<TItem>(items, execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Func<TItem, Task> execution, Func<TItem, Task> compensation)
            => await ForeachAsync<TItem>(items, execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Items + Execution + Compensation + CompensateAtTag Overloads
        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Action<TItem> compensation, Tag compensateAtTag)
            => await ForeachAsync<TItem>(items, execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Func<TItem, Task> compensation, Tag compensateAtTag)
            => await ForeachAsync<TItem>(items, execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);


        public async Task ForeachAsync<TItem>(IEnumerable<TItem> items, Func<TItem, Task> execution, Action<TItem> compensation, Tag compensateAtTag)
            => await ForeachAsync<TItem>(items, execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
