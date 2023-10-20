using System;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void Do(Action execution, Action compensation, Tag compensateAtTag)
        {
            Execute(
                validation: () =>
                {
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () =>
                {
                    execution();

                    if (compensation != null)
                        _compensationStack.AddCompensation(compensation, compensateAtTag);
                });
        }

        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void Do(Func<Compensation> execution, Tag compensateAtTag)
        {
            Execute(
                validation: () =>
                {
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () =>
                {
                    var executionCompensation = execution();
                    Validate.ExecutionCompensation(executionCompensation);

                    if (executionCompensation.HasCompensation)
                        _compensationStack.AddCompensation(executionCompensation.Compensate, compensateAtTag);
                });
        }

        #region Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        public void Do(Action execution)
            => Do(execution, default(Action), default(Tag));
        #endregion

        #region Compensable Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        public void Do(Func<Compensation> execution)
            => Do(execution, default(Tag));
        #endregion

        #region Execution + Compensation Overloads
        /// <summary>
        /// Runs the <i>execution</i>. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        public void Do(Action execution, Action compensation)
            => Do(execution, compensation, default(Tag));
        #endregion
    }
}