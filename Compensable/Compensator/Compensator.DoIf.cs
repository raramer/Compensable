using System;

namespace Compensable
{
    partial class Compensator
    {
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

        #region Test + Execution Overloads
        public void DoIf(bool test, Action execution)
            => DoIf(() => test, execution, default(Action), default(Tag));

        public void DoIf(Func<bool> test, Action execution)
            => DoIf(test, execution, default(Action), default(Tag));
        #endregion

        #region Test + Execution + Compensation Overloads
        public void DoIf(bool test, Action execution, Action compensation)
            => DoIf(() => test, execution, compensation, default(Tag));

        public void DoIf(Func<bool> test, Action execution, Action compensation)
            => DoIf(test, execution, compensation, default(Tag));
        #endregion

        #region Test + Execution + Compensation + CompensateAtTag Overloads
        public void DoIf(bool test, Action execution, Action compensation, Tag compensateAtTag)
            => DoIf(() => test, execution, compensation, compensateAtTag);
        #endregion
    }
}