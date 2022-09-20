using System;

namespace Compensable
{
    [Serializable]
    public class CompensationException : Exception
    {
        internal CompensationException(Exception whileCompensating)
            : base($"While compensating: {whileCompensating.Message}")
        {
            WhileCompensating = whileCompensating;
        }

        internal CompensationException(Exception whileCompensating, Exception whileExecuting)
            : base($"While executing: {whileExecuting.Message}{Environment.NewLine}While compensating: {whileCompensating.Message}")
        {
            WhileCompensating = whileCompensating;
            WhileExecuting = whileExecuting;
        }

        public Exception WhileExecuting { get; }
        public Exception WhileCompensating { get; }
    }
}