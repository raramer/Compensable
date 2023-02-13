using System;
using System.Collections.Generic;

namespace Compensable
{
    partial class Compensator
    {
        public void Foreach<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Action<TItem> compensation, Tag compensateAtTag)
        {
            Execute(
                validation: () =>
                {
                    Validate.Items(items);
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () =>
                {
                    // TODO do we need create a foreach specific Tag to guarantee all foreach item compensations are grouped?
                    foreach (var next in items)
                    {
                        // store item to local variable as the value of next is updated by the enumerator
                        var item = next;

                        execution(item);

                        if (compensation != null)
                            _compensationStack.AddCompensation(() => compensation(item), compensateAtTag);
                    }
                });
        }

        #region Items + Execution Overloads
        public void Foreach<TItem>(IEnumerable<TItem> items, Action<TItem> execution)
            => Foreach<TItem>(items, execution, default(Action<TItem>), default(Tag));
        #endregion

        #region Items + Execution + Compensation Overloads
        public void Foreach<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Action<TItem> compensation)
            => Foreach<TItem>(items, execution, compensation, default(Tag));
        #endregion
    }
}
