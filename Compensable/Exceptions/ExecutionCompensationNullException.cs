using System;
using System.Runtime.Serialization;

namespace Compensable
{
    [Serializable]
    public class ExecutionCompensationNullException : Exception
    {
        internal ExecutionCompensationNullException() : base("Execution's response compensation cannot be null.")
        { 
        }

        protected ExecutionCompensationNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
