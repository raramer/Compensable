using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        /// <summary>
        /// Creates a tagged position in the compensation stack.
        /// </summary>
        /// <returns>A task that represents creating the tag.</returns>
        public async Task<Tag> CreateTagAsync()
            => await CreateTagAsync(default(string)).ConfigureAwait(false);

        internal async Task<Tag> CreateTagAsync(string label)
            => await ExecuteAsync(() => _compensationStack.AddTag(label)).ConfigureAwait(false);
    }
}
