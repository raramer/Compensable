using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        public async Task CommitAsync()
            => await ExecuteAsync(_compensationStack.Clear).ConfigureAwait(false);
    }
}
