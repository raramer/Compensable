namespace Compensable
{
    partial class Compensator
    {
        /// <summary>
        /// Commits all previous executions by clearing the compensation stack.
        /// </summary>
        public void Commit()
            => Execute(_compensationStack.Clear);
    }
}
