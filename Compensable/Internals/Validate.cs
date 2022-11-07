using System;
using System.Collections.Generic;

namespace Compensable
{
    internal static class Validate
    {
        internal static void Compensation(Delegate compensation)
        {
            if (compensation == null)
                throw new ArgumentNullException(nameof(compensation));
        }

        internal static void Execution(Delegate execution)
        {
            if (execution == null)
                throw new ArgumentNullException(nameof(execution));
        }

        internal static void Items<T>(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
        }

        internal static void Test(Delegate test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));
        }
    }
}
