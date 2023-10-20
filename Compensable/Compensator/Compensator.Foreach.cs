using System;
using System.Collections.Generic;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Runs an <i>execution</i> for each enumerated <i>item</i>. If successful, an item specific <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items to enumerate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
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

        /// <summary>
        /// Runs an <i>execution</i> for each enumerated <i>item</i>. If successful, the returned item specific <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items to enumerate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void Foreach<TItem>(IEnumerable<TItem> items, Func<TItem, Compensation> execution, Tag compensateAtTag)
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

                        var executionCompensation = execution(item);
                        Validate.ExecutionCompensation(executionCompensation);

                        if (executionCompensation.HasCompensation)
                            _compensationStack.AddCompensation(executionCompensation.Compensate, compensateAtTag);
                    }
                });
        }

        #region Items + Execution Overloads
        /// <summary>
        /// Runs an <i>execution</i> for each enumerated <i>item</i>.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items to enumerate.</param>
        /// <param name="execution">The execution to run.</param>
        public void Foreach<TItem>(IEnumerable<TItem> items, Action<TItem> execution)
            => Foreach<TItem>(items, execution, default(Action<TItem>), default(Tag));
        #endregion

        #region Items + Compensable Execution Overloads
        /// <summary>
        /// Runs an <i>execution</i> for each enumerated <i>item</i>. If successful, the returned item specific <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items to enumerate.</param>
        /// <param name="execution">The execution to run.</param>
        public void Foreach<TItem>(IEnumerable<TItem> items, Func<TItem, Compensation> execution)
            => Foreach<TItem>(items, execution, default(Tag));
        #endregion

        #region Items + Execution + Compensation Overloads
        /// <summary>
        /// Runs an <i>execution</i> for each enumerated <i>item</i>. If successful, an item specific <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items to enumerate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        public void Foreach<TItem>(IEnumerable<TItem> items, Action<TItem> execution, Action<TItem> compensation)
            => Foreach<TItem>(items, execution, compensation, default(Tag));
        #endregion
    }
}
