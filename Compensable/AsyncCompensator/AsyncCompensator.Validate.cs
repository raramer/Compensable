using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compensable
{
    partial class AsyncCompensator
    {
        private void ValidateCompensation(Delegate compensation)
        {
            if (compensation == null)
                throw new ArgumentNullException(nameof(compensation));
        }

        private void ValidateExecution(Delegate execution)
        {
            if (execution == null)
                throw new ArgumentNullException(nameof(execution));
        }

        private void ValidateItems<T>(IEnumerable<T> items)
        { 
            if (items == null)
                throw new ArgumentNullException(nameof(items));
        }

        private void ValidateTag(Tag tag)
        {
            if (tag != null && !_taggedCompensations.Any(c => c.Tag == tag))
                throw new TagNotFoundException();
        }

        private void ValidateTest(Func<Task<bool>> test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
        }
    }
}