using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        public async Task ForeachAsync<T>(IEnumerable<T> items, Func<T, Task> execution, Func<T, Task> compensation, Tag compensateAtTag)
        {
            await ExecuteAsync(
                validation: () =>
                {
                    ValidateItems(items);
                    ValidateExecution(execution);
                    ValidateTag(compensateAtTag);
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
                            AddCompensationToStack(async () => await compensation(item).ConfigureAwait(false), compensateAtTag);
                    }
                }).ConfigureAwait(false);
        }

        #region Items + Execution Overloads
        public async Task ForeachAsync<T>(IEnumerable<T> items, Action<T> execution)
            => await ForeachAsync<T>(items, execution.Awaitable(), default(Func<T, Task>), default(Tag)).ConfigureAwait(false);

        public async Task ForeachAsync<T>(IEnumerable<T> items, Func<T, Task> execution)
            => await ForeachAsync<T>(items, execution, default(Func<T, Task>), default(Tag)).ConfigureAwait(false);
        #endregion

        #region Items + Execution + Compensation Overloads
        public async Task ForeachAsync<T>(IEnumerable<T> items, Action<T> execution, Action<T> compensation)
            => await ForeachAsync<T>(items, execution.Awaitable(), compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task ForeachAsync<T>(IEnumerable<T> items, Action<T> execution, Func<T, Task> compensation)
            => await ForeachAsync<T>(items, execution.Awaitable(), compensation, default(Tag)).ConfigureAwait(false);


        public async Task ForeachAsync<T>(IEnumerable<T> items, Func<T, Task> execution, Action<T> compensation)
            => await ForeachAsync<T>(items, execution, compensation.Awaitable(), default(Tag)).ConfigureAwait(false);

        public async Task ForeachAsync<T>(IEnumerable<T> items, Func<T, Task> execution, Func<T, Task> compensation)
            => await ForeachAsync<T>(items, execution, compensation, default(Tag)).ConfigureAwait(false);
        #endregion

        #region Items + Execution + Compensation + CompensateAtTag Overloads
        public async Task ForeachAsync<T>(IEnumerable<T> items, Action<T> execution, Action<T> compensation, Tag compensateAtTag)
            => await ForeachAsync<T>(items, execution.Awaitable(), compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);

        public async Task ForeachAsync<T>(IEnumerable<T> items, Action<T> execution, Func<T, Task> compensation, Tag compensateAtTag)
            => await ForeachAsync<T>(items, execution.Awaitable(), compensation, compensateAtTag).ConfigureAwait(false);


        public async Task ForeachAsync<T>(IEnumerable<T> items, Func<T, Task> execution, Action<T> compensation, Tag compensateAtTag)
            => await ForeachAsync<T>(items, execution, compensation.Awaitable(), compensateAtTag).ConfigureAwait(false);
        #endregion
    }
}
