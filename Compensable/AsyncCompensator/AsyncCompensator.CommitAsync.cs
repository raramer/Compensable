using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        /// <summary>
        /// Commits all previous executions by clearing the compensation stack.
        /// </summary>
        /// <returns>A task that represents clearing the compensation stack.</returns>
        public async Task CommitAsync()
            => await ExecuteAsync(_compensationStack.Clear).ConfigureAwait(false);
    }
}
