using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task CommitAsync()
            => await ExecuteAsync(ClearStack).ConfigureAwait(false);
    }
}
