using System;

namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Adds a <i>compensation</i> to a tagged position in the compensation stack without requiring a successfully completed execution.
        /// </summary>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        /// <param name="compensateAtTag">A tagged position in the compensation stack.</param>
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
        /// <summary>
        /// Adds a <i>compensation</i> to the compensation stack without requiring a successfully completed execution.
        /// </summary>
        /// <param name="compensation">The compensation to add to the compensation stack.</param>
        public void AddCompensation(Action compensation)
            => AddCompensation(compensation, default(Tag));
        #endregion
    }
}
