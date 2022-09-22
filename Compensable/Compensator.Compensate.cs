using System.Threading.Tasks;
using System;

namespace Compensable
{
    partial class Compensator
    {
        public async Task CompensateAsync() => await CompensateAsync(null).ConfigureAwait(false);
    }
}
