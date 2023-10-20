using System;

namespace Compensable
{
    public sealed class Compensation : IExecutionCompensation
    {
        private readonly Action _compensation;

        internal bool HasCompensation => _compensation != null;

        public Compensation(Action compensation)
        {
            _compensation = compensation;
        }

        public void Compensate()
        { 
            if (HasCompensation)
                _compensation();
        }

        public static Compensation Noop => new Compensation(default(Action));
    }
}
