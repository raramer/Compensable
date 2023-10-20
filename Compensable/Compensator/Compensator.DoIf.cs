using System;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void DoIf(Func<bool> test, Action execution, Action compensation, Tag compensateAtTag)
        {
            Execute(
                validation: () =>
                {
                    Validate.Test(test);
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () =>
                {
                    if (test())
                    {
                        execution();

                        if (compensation != null)
                            _compensationStack.AddCompensation(compensation, compensateAtTag);
                    }
                });
        }

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void DoIf(Func<bool> test, Func<Compensation> execution, Tag compensateAtTag)
        {
            Execute(
                validation: () =>
                {
                    Validate.Test(test);
                    Validate.Execution(execution);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () =>
                {
                    if (test())
                    {
                        var executionCompensation = execution();
                        Validate.ExecutionCompensation(executionCompensation);

                        if (executionCompensation.HasCompensation)
                            _compensationStack.AddCompensation(executionCompensation.Compensate, compensateAtTag);
                    }
                });
        }

        #region Test + Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        public void DoIf(bool test, Action execution)
            => DoIf(() => test, execution, default(Action), default(Tag));

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        public void DoIf(Func<bool> test, Action execution)
            => DoIf(test, execution, default(Action), default(Tag));
        #endregion

        #region Test + Compensable Execution Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        public void DoIf(bool test, Func<Compensation> execution)
            => DoIf(() => test, execution, default(Tag));

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        public void DoIf(Func<bool> test, Func<Compensation> execution)
            => DoIf(test, execution, default(Tag));
        #endregion

        #region Test + Execution + Compensation Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        public void DoIf(bool test, Action execution, Action compensation)
            => DoIf(() => test, execution, compensation, default(Tag));

        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        public void DoIf(Func<bool> test, Action execution, Action compensation)
            => DoIf(test, execution, compensation, default(Tag));
        #endregion

        #region Test + Execution + Compensation + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the <i>compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void DoIf(bool test, Action execution, Action compensation, Tag compensateAtTag)
            => DoIf(() => test, execution, compensation, compensateAtTag);
        #endregion

        #region Test + Compensable Execution + CompensateAtTag Overloads
        /// <summary>
        /// Runs the <i>execution</i> only if <i>test</i> evaluates to true. If successful, the returned <i>Compensation</i> is added to a tagged position in the compensation stack.
        /// </summary>
        /// <param name="test">The test to evaluate.</param>
        /// <param name="execution">The execution to run.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
        public void DoIf(bool test, Func<Compensation> execution, Tag compensateAtTag)
            => DoIf(() => test, execution, compensateAtTag);
        #endregion
    }
}