using System;

namespace Compensable
{
    public sealed class Tag
    {
        public string Label { get; }

        internal Tag() : this(null)
        {
        }
        internal Tag(string label)
        { 
            Label = string.IsNullOrWhiteSpace(label)
                ? Guid.NewGuid().ToString("n")
                : label.Trim();
        }
    }
}
