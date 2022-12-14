using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        public async Task<Tag> CreateTagAsync()
            => await CreateTagAsync(default(string)).ConfigureAwait(false);

        internal async Task<Tag> CreateTagAsync(string label)
            => await ExecuteAsync(() => _compensationStack.AddTag(label)).ConfigureAwait(false);
    }
}
