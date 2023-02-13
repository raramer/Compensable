using System;

namespace Compensable
{
    partial class Compensator
    {
        public void AddCompensation(Action compensation, Tag compensateAtTag)
        {
            Execute(
                validation: () =>
                {
                    Validate.Compensation(compensation);
                    _compensationStack.ValidateTag(compensateAtTag);
                },
                execution: () => _compensationStack.AddCompensation(compensation, compensateAtTag)
            );
        }

        #region Compensation Overloads
        public void AddCompensation(Action compensation)
            => AddCompensation(compensation, default(Tag));
        #endregion
    }
}
