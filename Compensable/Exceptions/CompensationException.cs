using System;
using System.Runtime.Serialization;

namespace Compensable
{
    [Serializable]
    public class CompensationException : Exception
    {
        public Exception WhileCompensating { get; }

        public Exception WhileExecuting { get; }

        internal CompensationException(Exception whileCompensating)
            : base($"While compensating: {whileCompensating.Message}", innerException: whileCompensating)
        {
            WhileCompensating = whileCompensating;
        }

        internal CompensationException(Exception whileCompensating, Exception whileExecuting)
            : base($"While executing: {whileExecuting.Message}{Environment.NewLine}While compensating: {whileCompensating.Message}", innerException: whileExecuting)
        {
            WhileCompensating = whileCompensating;
            WhileExecuting = whileExecuting;
        }

        protected CompensationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // TODO 
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // TODO
            base.GetObjectData(info, context);
        }
    }
}