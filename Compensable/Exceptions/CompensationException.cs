using System;
using System.Runtime.Serialization;

namespace Compensable
{
    [Serializable]
    public class CompensationException : Exception
    {
        /// <summary>
        /// The exception that was thrown while compensating.
        /// </summary>
        public Exception WhileCompensating { get; }

        /// <summary>
        /// The exception that was thrown while executing.
        /// </summary>
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