using System.Threading.Tasks;

namespace Compensable
{
    partial class Compensator
    {
        public async Task CompensateAsync() => await CompensateAsync(null).ConfigureAwait(false);
    }
}
