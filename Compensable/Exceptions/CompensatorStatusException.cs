using System;
using System.Runtime.Serialization;

namespace Compensable
{
    [Serializable]
    public class CompensatorStatusException : Exception
    {
        internal CompensatorStatusException(CompensatorStatus status) : base($"Compensator status is {status}")
        {
        }

        protected CompensatorStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}