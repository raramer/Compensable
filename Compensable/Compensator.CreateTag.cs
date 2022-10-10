using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task<Tag> CreateTagAsync()
            => await CreateTagAsync(default(string)).ConfigureAwait(false);

        internal async Task<Tag> CreateTagAsync(string label)
            => await ExecuteAsync(() => AddTagToStack(new Tag(label))).ConfigureAwait(false);
    }
}
