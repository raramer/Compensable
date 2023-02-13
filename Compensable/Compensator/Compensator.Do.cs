using System;

namespace Compensable
{
    partial class Compensator
    {
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

        #region Execution Overloads
        public void Do(Action execution)
            => Do(execution, default(Action), default(Tag));
        #endregion

        #region Execution + Compensation Overloads
        public void Do(Action execution, Action compensation)
            => Do(execution, compensation, default(Tag));
        #endregion
    }
}