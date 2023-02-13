using System;

namespace Compensable
{
    partial class Compensator
    {
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
        public TResult Get<TResult>(Func<TResult> execution)
            => Get(execution, default(Action<TResult>), default(Tag));
        #endregion

        #region Execution + Compensation Overloads
        public TResult Get<TResult>(Func<TResult> execution, Action compensation)
            => Get(execution, compensation.IgnoreParameter<TResult>(), default(Tag));

        public TResult Get<TResult>(Func<TResult> execution, Action<TResult> compensation)
            => Get(execution, compensation, default(Tag));
        #endregion

        #region Execution + Compensation + CompensateAtTag Overloads
        public TResult Get<TResult>(Func<TResult> execution, Action compensation, Tag tag)
            => Get(execution, compensation.IgnoreParameter<TResult>(), tag);
        #endregion
    }
}
