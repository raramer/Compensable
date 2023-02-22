using System;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>The result of the execution.</returns>
        public TResult Get<TResult>(Func<TResult> execution, Action<TResult> compensation, Tag compensateAtTag)
        {
            return Execute(
                validation: () =>
                {
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () =>
                {
                    var result = execution();

                    if (compensation != null)
                        _compensationStack.AddCompensation(() => compensation(result), compensateAtTag);

                    return result;
                });
        }

        #region Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <returns>The result of the execution.</returns>
        public TResult Get<TResult>(Func<TResult> execution)
            => Get(execution, default(Action<TResult>), default(Tag));
        #endregion

        #region Execution + Compensation Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>The result of the execution.</returns>
        public TResult Get<TResult>(Func<TResult> execution, Action compensation)
            => Get(execution, compensation.IgnoreParameter<TResult>(), default(Tag));

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <returns>The result of the execution.</returns>
        public TResult Get<TResult>(Func<TResult> execution, Action<TResult> compensation)
            => Get(execution, compensation, default(Tag));
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        /// <returns>The result of the execution.</returns>
        public TResult Get<TResult>(Func<TResult> execution, Action compensation, Tag tag)
            => Get(execution, compensation.IgnoreParameter<TResult>(), tag);
        #endregion
    }
}
