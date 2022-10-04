using System;
using System.Runtime.Serialization;

namespace Compensable
{
    [Serializable]
    public class CompensationException : Exception
    {
        public Exception WhileCompensating { get; }

        public Exception WhileExecuting { get; }

        internal CompensationException(Exception whileCompensating, Exception whileExecuting) : base(
            message: whileExecuting == null
                ? $"While compensating: {whileCompensating?.Message}"
                : $"While executing: {whileExecuting.Message}{Environment.NewLine}While compensating: {whileCompensating?.Message}",
            innerException: whileExecuting ?? whileCompensating)
        {
            WhileCompensating = whileCompensating;
            WhileExecuting = whileExecuting;
        }

        protected CompensationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}