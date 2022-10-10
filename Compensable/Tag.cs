using System;

namespace Compensable
{
    public sealed class Tag
    {
        internal string Label { get; } // TODO remove this?

        internal Tag() : this(null)
        {
        }

        internal Tag(string label) // TODO remove this?
        {
            Label = string.IsNullOrWhiteSpace(label)
                ? Guid.NewGuid().ToString("n")
                : label.Trim();
        }
    }
}
