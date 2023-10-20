using System;

namespace Compensable
{
    public sealed class Compensation<TResult> : IExecutionCompensation
    {
        private Action _compensation { get; }

        internal bool HasCompensation => _compensation != null;

        public TResult Result { get; }

        public Compensation(TResult result, Action compensation)
        {
            Result = result;
            _compensation = compensation;
        }

        public Compensation(TResult result, Action<TResult> compensation)
        {
            Result = result;
            _compensation = compensation == null 
                ? default(Action) : 
                () => compensation(result);
        }

        public void Compensate()
        {
            if (HasCompensation)
                _compensation();
        }

        public static Compensation<TResult> Noop(TResult result) => new Compensation<TResult>(result, default(Action));

        public static implicit operator TResult(Compensation<TResult> compensableResponse) => compensableResponse.Result;
    }
}
