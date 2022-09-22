using System;
using System.Runtime.Serialization;

namespace Compensable
{
    [Serializable]
    public class TagNotFoundException : Exception
    {
        internal TagNotFoundException() : base("Tag not found")
        {
        }

        protected TagNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}