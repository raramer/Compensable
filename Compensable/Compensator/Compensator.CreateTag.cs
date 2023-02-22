namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Creates a tagged position in the compensation stack.
        /// </summary>
        public Tag CreateTag()
            => CreateTag(default(string));

        internal Tag CreateTag(string label)
            => Execute(() => _compensationStack.AddTag(label));
    }
}
