namespace Compensable
{
    partial class Compensator
    {
        public Tag CreateTag()
            => CreateTag(default(string));

        internal Tag CreateTag(string label)
            => Execute(() => _compensationStack.AddTag(label));
    }
}
